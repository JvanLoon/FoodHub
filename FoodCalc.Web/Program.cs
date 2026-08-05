using Blazored.LocalStorage;
using FoodCalc.Web.App;
using FoodCalc.Web.Components;
using FoodCalc.Web.Services;
using FoodCalc.Web.Services.Admin;
using FoodCalc.Web.Services.Auth;
using FoodCalc.Web.Services.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;

public class Program
{
    public static void Main(string[] args)
    {
        // See the matching block in FoodCalc.Api/Program.cs. NoClobber keeps real
        // environment variables authoritative; absent in a container, so it no-ops.
        if (File.Exists(".env"))
            DotNetEnv.Env.NoClobber()
                .Load();

        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddHubOptions(options =>
            {
                options.MaximumReceiveMessageSize = ImportExportSettings.DefaultMaxFileSizeInBytes;
            });

        builder.Services.AddOutputCache();

        builder.Services.AddBlazoredLocalStorage();

        var apiBaseAddress = builder.Configuration["API:BaseAddress"];

        if (string.IsNullOrEmpty(apiBaseAddress))
            throw new InvalidOperationException("API base address is not configured.");

        builder.Services.AddHttpClient("ApiClient", client => { client.BaseAddress = new Uri(apiBaseAddress); });

        var keysPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FoodHub", "dataprotection-keys");
        Directory.CreateDirectory(keysPath);

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
            .SetApplicationName("FoodHub");

        // The browser's only credential. It carries the JWT as an encrypted claim (see
        // AuthCookie), so the token itself never reaches the browser and no script can read it.
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = AuthCookie.CookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                // Strict, so a cross-site request carries no cookie at all. That is what lets
                // /logout be a plain GET the circuit can navigate to: a forged cross-site
                // navigation to it arrives unauthenticated and signs nobody out.
                //
                // The cost is the first hop in from an external link — a mail, a chat, a search
                // result — which arrives without the cookie and therefore lands on the login page.
                // The session is not gone; the next navigation inside the site is same-site and
                // sends the cookie again. Lax trades that back for a logout that must be a POST.
                options.Cookie.SameSite = SameSiteMode.Strict;

                options.LoginPath = AuthCookie.LoginPath;
                options.LogoutPath = AuthCookie.LogoutPath;
                options.AccessDeniedPath = "/";

                // Every ticket carries an explicit ExpiresUtc taken from the JWT's own exp, so
                // this is only the fallback for a ticket that somehow has none. Not sliding: the
                // JWT cannot be renewed short of the password, so extending the cookie past it
                // would leave the UI looking signed in while every API call 401s.
                options.ExpireTimeSpan = TimeSpan.FromHours(12);
                options.SlidingExpiration = false;
            });

        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<TokenProvider>();
        builder.Services.AddScoped<PresenceService>();
        builder.Services.AddScoped<AuthStateService>();
        builder.Services.AddScoped<AdminService>();
        builder.Services.AddScoped<LoginService>();
        builder.Services.AddScoped<RecipeService>();
        builder.Services.AddScoped<ReviewService>();
        builder.Services.AddScoped<MealPlanService>();
        builder.Services.AddScoped<IngredientService>();
        builder.Services.AddScoped<ImportExportService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<MessageService>();

        builder.Services.AddScoped<AuthenticatedHttpClientService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenProvider = sp.GetRequiredService<TokenProvider>();
            var httpClient = httpClientFactory.CreateClient("ApiClient");
            return new AuthenticatedHttpClientService(httpClient, tokenProvider,
                sp.GetRequiredService<ILogger<AuthenticatedHttpClientService>>(),
                sp.GetRequiredService<MessageService>(), sp.GetRequiredService<NavigationManager>());
        });

        // CORS config
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAPI", builder2 => builder2.WithOrigins(apiBaseAddress)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });

        // See the matching block in FoodCalc.Api/Program.cs. This matters more here than
        // on the API: Blazor Server negotiates its SignalR circuit against an absolute
        // URL, and without the forwarded scheme it would try ws:// from an https page
        // and be blocked as mixed content.
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.IncludeFields = false;
            //options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            //options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        // Still no JWT *validation* here, and still deliberately no signing key in this process.
        //
        // The tokens are HS256, so the key does not merely verify them — it mints them. A
        // process holding it can forge a token for any account with any role. This one is the
        // internet-facing container (the tunnel points at web; api publishes no ports at all),
        // so keeping the key here handed the exposed process the ability to forge credentials
        // for the one that was deliberately kept unreachable.
        //
        // The cookie scheme above does not change that. It authenticates the *browser* to this
        // app with a ticket this app minted and Data Protection signed; the JWT rides inside as
        // an opaque string that only ever gets read for its claims (ReadJwtToken, no signature
        // check, no key) and forwarded to the API, which validates it properly.

        var app = builder.Build();

        var behindProxy = app.Configuration.GetValue("ReverseProxy:Enabled", !app.Environment.IsDevelopment());

        // Must run before anything that reads the scheme or the client IP.
        if (behindProxy)
            app.UseForwardedHeaders();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // The edge proxy already forces https in Production; redirecting again inside
        // the container only breaks its plain-HTTP health probe.
        if (!behindProxy)
            app.UseHttpsRedirection();

        // Removed in 95118a6 because nothing consumed the principal they produced. The cookie
        // scheme is what earns them back: UseAuthentication is what turns the cookie into
        // HttpContext.User, which is where every role check and the JWT itself now come from.
        // Both must run before UseAntiforgery, which validates per-endpoint against that user.
        app.UseAuthentication();
        app.UseAuthorization();

        // Both work off the principal UseAuthentication produced, and in this order: the hint
        // decides whether an unauthenticated request deserves a second chance, and the redirect
        // only gives up on the ones that do not.
        app.UseSessionHint();
        app.UseLoginRedirect();

        app.UseAntiforgery();

        //app.UseOutputCache();
        app.UseCors("AllowAPI");

        // Replaces UseStaticFiles: serves wwwroot from the build-time asset manifest, which is
        // what makes the fingerprinted @Assets[..] URLs in App.razor resolve. It also sets
        // immutable, long-lived caching on those hashed URLs — safe precisely because the URL
        // changes whenever the content does.
        app.MapStaticAssets();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Signing out has to happen in a real HTTP request: a cookie cannot be cleared over the
        // SignalR circuit, because there are no response headers to clear it in. A GET rather
        // than a form post so the circuit can simply navigate here — safe because the cookie is
        // SameSite=Strict, so a cross-site navigation to this route arrives with no session to
        // end. Marking the account offline is the caller's job and happens before the trip here,
        // while the JWT is still usable.
        app.MapGet(AuthCookie.LogoutPath, async (HttpContext http) =>
        {
            await AuthCookie.SignOutAsync(http);
            return Results.Redirect(AuthCookie.LoginPath);
        });

        app.MapDefaultEndpoints();

        app.Run();
    }
}