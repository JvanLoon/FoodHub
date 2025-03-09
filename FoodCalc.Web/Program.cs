using FoodCalc.Web.Components;
using FoodCalc.Web.Services;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddOutputCache();

//builder.Services.AddHttpClient("FoodCalcApi", client =>
//{
//	// This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
//	// Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
//	client.BaseAddress = new Uri("https://localhost:7428");
//});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7428") });

builder.Services.Configure<JsonOptions>(options =>
{
	options.SerializerOptions.IncludeFields = false;
	//options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddScoped<ReceptService>();

builder.Services.AddCustomServices();

builder.Services.AddApplicationMediatR();

// Register ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
