using FastEndpoints;
using FastEndpoints.Swagger;
using FoodCalc.Api.Extensions;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodCalc.Api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // Local development config lives in a gitignored .env next to the csproj
        // (see .env.example). NoClobber means a real environment variable always
        // wins, so Aspire's injected connection string and the values docker
        // compose sets are untouched — and in a container, where there is no .env,
        // this is a no-op.
        if (File.Exists(".env"))
            DotNetEnv.Env.NoClobber()
                .Load();

        var builder = WebApplication.CreateBuilder(args);

        var webBaseAddress = builder.Configuration["WebServer:BaseAddress"];
        if (string.IsNullOrEmpty(webBaseAddress))
            throw new InvalidOperationException("Web server base address is not configured.");

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddProblemDetails();

        builder.Services.AddFastEndpoints();
        builder.Services.SwaggerDocument();

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.IncludeFields = false;
            options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.Converters.Add(new DateTimeConverter());
            //options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        // Add services to the container.
        // Aspire injects the Postgres connection string under the resource name
        // ("foodcalc"); the docker-compose stack and a plain local run supply it
        // as "DefaultConnection". Prefer the Aspire one, fall back to the latter.
        var connectionString = builder.Configuration.GetConnectionString("foodcalc") ??
                               builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("No database connection string is configured.");

        builder.Services.AddDbContext<FoodHubDbContext>(options => options.UseNpgsql(connectionString));

        // Add Identity
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<FoodHubDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin,Moderator,User", policy => policy.RequireRole("Admin", "Moderator", "User"));

            options.AddPolicy("Admin,Moderator", policy => policy.RequireRole("Admin", "Moderator"));

            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));

            options.AddPolicy("Moderator", policy => policy.RequireRole("Moderator"));

            options.AddPolicy("User", policy => policy.RequireRole("User"));
        });

        // Configure JWT authentication
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
        string key = jwtSettings?.Key;

        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("JWT key is not configured. Set Jwt__Key in the environment (see deployment.md).");

        // HMAC-SHA256 keys shorter than the 256-bit hash add no security and make the
        // signature trivially brute-forceable. Enforced everywhere, not just Production,
        // so a too-short key fails on the developer's machine rather than on the server.
        if (System.Text.Encoding.UTF8.GetByteCount(key) < 32)
            throw new InvalidOperationException(
                "JWT key is too short: it must be at least 32 bytes. Generate one with `openssl rand -base64 48`.");

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidAudience = jwtSettings?.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings?.Issuer,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
                };

                options.SaveToken = true;
            });

        // Behind a TLS-terminating reverse proxy the container only ever speaks plain
        // HTTP, so honour the proxy's X-Forwarded-* headers — otherwise ASP.NET Core
        // sees scheme "http" and emits redirects and links on the wrong origin.
        // KnownNetworks/KnownProxies are cleared because the proxy is a separate
        // container, not loopback, and the default allowlist would drop the headers.
        // Safe only while the API is unreachable except through that proxy: the
        // production compose file must not publish this container's port.
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // Use the custom service registration method
        builder.Services.AddApplicationMediatR();

        // CORS config
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowWebApp", builder2 => builder2.WithOrigins(webBaseAddress)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });

        var app = builder.Build();

        var behindProxy = app.Configuration.GetValue("ReverseProxy:Enabled", !app.Environment.IsDevelopment());

        // Schema first, then the roles/first-admin bootstrap that replaced the old
        // HasData seed. Both are idempotent; either one throwing takes the process
        // down on purpose, because serving traffic against a half-built database is
        // worse than a visible crash loop.
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FoodHubDbContext>();
            await db.Database.MigrateAsync();
        }

        await app.BootstrapIdentityAsync();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

        // In Development the API is its own https endpoint and redirects itself. In
        // Production the edge proxy already forces https, and an in-container redirect
        // would only bounce the internal http call from the web front end.
        if (behindProxy)
            app.UseForwardedHeaders();
        else
            app.UseHttpsRedirection();

        //app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("AllowWebApp");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseFastEndpoints(c =>
        {
            // FastEndpoints honors the global Microsoft.AspNetCore.Http.Json
            // options, which enable ReferenceHandler.Preserve. That serializes
            // collections as { "$id":.., "$values":[..] }, which the Blazor
            // clients cannot deserialize into PagedResultDto<T>. The old MVC
            // controllers serialized via Mvc.JsonOptions (plain web defaults, no
            // Preserve), so match that wire format to keep the clients working.
            c.Serializer.Options.ReferenceHandler = null;
            c.Serializer.Options.WriteIndented = false;

            // One error shape for everything: validator failures and domain failures
            // (see ErrorResultExtensions) both serialize as RFC9457 ProblemDetails with a
            // flat "errors": [{ name, reason }] array, so the client parses one thing.
            c.Errors.UseProblemDetails(p =>
            {
                // Domain errors all share the same field name; without this only the
                // first of them would survive into the response.
                p.AllowDuplicateErrors = true;
            });
        });
        if (app.Environment.IsDevelopment())
        {
            // FastEndpoints/NSwag Swagger is now the only Swagger provider, served
            // at the conventional /swagger.
            app.UseSwaggerGen();
        }

        app.MapDefaultEndpoints();

        await app.RunAsync();
    }
}

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}