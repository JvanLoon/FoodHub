using Blazored.LocalStorage;

using FoodCalc.Web.Components;
using FoodCalc.Web.Components.Services;

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
			.AddInteractiveServerComponents();

		builder.Services.AddOutputCache();

		var apiBaseAddress = builder.Configuration["API:BaseAddress"];
		builder.Services.AddTransient<AuthTokenHandler>();
		builder.Services.AddHttpClient("ApiClient", client =>
		{
			client.BaseAddress = new Uri(apiBaseAddress);
		}).AddHttpMessageHandler<AuthTokenHandler>();

		builder.Services.AddBlazoredLocalStorage();

		builder.Services.AddDataProtection()
		.PersistKeysToFileSystem(new DirectoryInfo(@"/root/.aspnet/DataProtection-Keys"))
		.SetApplicationName("FoodHub");

		builder.Services.AddScoped<RecipeService>();
		builder.Services.AddScoped<IngredientService>();
		builder.Services.AddScoped<LoginService>();
		builder.Services.AddScoped<AdminService>();
		builder.Services.AddScoped<AuthTokenService>();

		builder.Services.AddSingleton<AggregatedIngredientService>();

		builder.Services.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.IncludeFields = false;
			//options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
			//options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		});

		builder.Services.ConfigureApplicationCookie(options =>
		 {
			 options.Cookie.SameSite = SameSiteMode.None;
			 options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // requires HTTPS
		 });

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

		app.UseOutputCache();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		app.MapDefaultEndpoints();

		app.Run();

	}
}
