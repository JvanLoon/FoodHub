using Blazored.LocalStorage;

using FoodCalc.Web.Components;
using FoodCalc.Web.Components.Services;
using FoodCalc.Web.Components.Services.Admin;
using FoodCalc.Web.Components.Services.Auth;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Json;

public class Program
{
	public static void Main(string[] args)
	{
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
		builder.Services.AddHttpClient("ApiClient", client =>
		{
			client.BaseAddress = new Uri(apiBaseAddress);
		});

		builder.Services.AddDataProtection()
		.PersistKeysToFileSystem(new DirectoryInfo(@"/root/.aspnet/DataProtection-Keys"))
		.SetApplicationName("FoodHub");

		builder.Services.AddScoped<AuthTokenService>();
		builder.Services.AddScoped<AuthStateService>();
		builder.Services.AddScoped<AdminService>();
		builder.Services.AddScoped<LoginService>();
		builder.Services.AddScoped<RecipeService>();
		builder.Services.AddScoped<IngredientService>();
		builder.Services.AddScoped<ImportExportService>();
		builder.Services.AddScoped<UserService>();
		builder.Services.AddScoped<MessageService>();

		builder.Services.AddScoped<AuthenticatedHttpClientService>(sp =>
		{
			var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
			var authTokenService = sp.GetRequiredService<AuthTokenService>();
			var httpClient = httpClientFactory.CreateClient("ApiClient");
			return new AuthenticatedHttpClientService(httpClient, authTokenService);
		});

		// CORS config
		builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowAPI",
				builder => builder
					.WithOrigins(apiBaseAddress)
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials());
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
			var jwtSettings = builder.Configuration.GetSection("Jwt");
			options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = false,
				ValidAudience = jwtSettings["Audience"],
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtSettings["Issuer"],
				IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
					System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"])
				),
				ClockSkew = TimeSpan.Zero
			};

			options.SaveToken = true;

			options.Events = new JwtBearerEvents
			{
				//Allows you to hook into JWT authentication events (e.g., for logging, custom validation).
				OnAuthenticationFailed = context => {  return Task.CompletedTask; }
			};
		});
		builder.Services.AddAuthorization();

		var app = builder.Build();

		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error", createScopeForErrors: true);
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

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
