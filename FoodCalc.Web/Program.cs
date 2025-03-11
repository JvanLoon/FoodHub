using FoodCalc.Web;
using FoodCalc.Web.Components;
using FoodCalc.Web.Services;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddServerSideBlazor();

//builder.Services.AddHttpClient("FoodCalcApi", client =>
//{
//	// This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
//	// Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
//	client.BaseAddress = new Uri("https://localhost:7428");
//});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7428") });

builder.Services.AddScoped<ReceptService>();

builder.Services.Configure<JsonOptions>(options =>
{
	options.SerializerOptions.IncludeFields = false;
	//options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
