using Blazored.LocalStorage;
using FoodCalc.Web.App;
using FoodCalc.Web.Components;
using FoodCalc.Web.Services;
using FoodCalc.Web.Services.Admin;
using FoodCalc.Web.Services.Auth;
using FoodCalc.Web.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        builder.Services.AddScoped<AuthTokenService>();
        builder.Services.AddScoped<AuthStateService>();
        builder.Services.AddScoped<AdminService>();
        builder.Services.AddScoped<LoginService>();
        builder.Services.AddScoped<RecipeService>();
        builder.Services.AddScoped<MealPlanService>();
        builder.Services.AddScoped<IngredientService>();
        builder.Services.AddScoped<ImportExportService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<MessageService>();

        builder.Services.AddScoped<AuthenticatedHttpClientService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var authTokenService = sp.GetRequiredService<AuthTokenService>();
            var httpClient = httpClientFactory.CreateClient("ApiClient");
            return new AuthenticatedHttpClientService(httpClient, authTokenService,
                sp.GetRequiredService<ILogger<AuthenticatedHttpClientService>>(),
                sp.GetRequiredService<MessageService>());
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

        builder.Services.AddAuthentication("JwtBearer")
            .AddJwtBearer("JwtBearer", options =>
            {
                var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
                string? key = jwtSettings?.Key;

                if (string.IsNullOrWhiteSpace(key))
                    throw new InvalidOperationException("JWT key is not configured.");

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidAudience = jwtSettings?.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings?.Issuer,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
                };

                options.SaveToken = true;

                options.Events = new JwtBearerEvents
                {
                    //Allows you to hook into JWT authentication events (e.g., for logging, custom validation).
                    OnAuthenticationFailed = context => { return Task.CompletedTask; }
                };
            });
        builder.Services.AddAuthorization();

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

        app.UseStaticFiles();
        app.UseAntiforgery();

        //app.UseOutputCache();
        app.UseCors("AllowAPI");

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapDefaultEndpoints();

        app.Run();
    }
}

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}