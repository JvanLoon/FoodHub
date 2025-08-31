using Blazored.LocalStorage;

using FoodCalc.Web.Components;
using FoodCalc.Web.Components.Services;
using FoodCalc.Web.Components.Services.Admin;
using FoodCalc.Web.Components.Services.Auth;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Json;

using Microsoft.AspNetCore.Authentication.JwtBearer;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add service defaults & Aspire client integrations.
		builder.AddServiceDefaults();

		// Add services to the container.
		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();

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
		builder.Services.AddScoped<AdminService>();
		builder.Services.AddScoped<LoginService>();
		builder.Services.AddScoped<RecipeService>();
		builder.Services.AddScoped<IngredientService>();
		builder.Services.AddSingleton<AggregatedIngredientService>();

		builder.Services.AddScoped<AuthenticatedHttpClientService>(sp =>
		{
			var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
			var localStorage = sp.GetRequiredService<Blazored.LocalStorage.ILocalStorageService>();
			var httpClient = httpClientFactory.CreateClient("ApiClient");
			return new AuthenticatedHttpClientService(httpClient, localStorage);
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
			options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = false, // set to true and specify ValidAudience if needed
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = builder.Configuration["Jwt:Issuer"],
				IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
					System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
				),
				ClockSkew = TimeSpan.Zero
			};
			options.TokenValidationParameters.ValidAudience = "your-api-audience";
			options.TokenValidationParameters.ValidateAudience = true;

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

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		app.MapDefaultEndpoints();

		app.Run();

	}
}
