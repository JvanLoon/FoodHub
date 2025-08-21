using FoodCalc.Web.Components;
using FoodCalc.Web.Components.Services;

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

		//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7426") });

		var apiBaseAddress = builder.Configuration["API:BaseAddress"] ?? "https://localhost:7426";
		builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });


		builder.Services.AddScoped<RecipeService>();
		builder.Services.AddScoped<IngredientService>();

		builder.Services.AddSingleton<AggregatedIngredientService>();

		builder.Services.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.IncludeFields = false;
			//options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
			//options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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
