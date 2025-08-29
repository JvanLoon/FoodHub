using FoodCalc.Api.Extensions;
using FoodCalc.Features.Mapping;

using FoodHub.Persistence.Entities;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		var apiBaseAddress = builder.Configuration["API:BaseAddress"] ?? "https://localhost:7426";

		// Add service defaults & Aspire client integrations.
		builder.AddServiceDefaults();

		// Add services to the container.
		builder.Services.AddProblemDetails();

		builder.Services.AddControllers();

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		// Add AutoMapper
		builder.Services.AddAutoMapper(typeof(MappingProfile));

		builder.Services.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.IncludeFields = false;
			options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
			options.SerializerOptions.WriteIndented = true;
			options.SerializerOptions.Converters.Add(new DateTimeConverter());
			//options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		});

		builder.Services.AddIdentity<User, IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		// Add services to the container.
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

		// Use the custom service registration method
		builder.Services.AddCustomServices();

		builder.Services.AddApplicationMediatR();

		builder.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(policy =>
			{
				policy.WithOrigins(apiBaseAddress) // your frontend origin
					  .AllowAnyHeader()
					  .AllowAnyMethod()
					  .AllowCredentials();
			});
		});

		builder.Services.ConfigureApplicationCookie(options =>
		{
			options.Cookie.SameSite = SameSiteMode.None;
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // requires HTTPS
		});

		var app = builder.Build();

		// Run migrations at startup
		using (var scope = app.Services.CreateScope())
		{
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			db.Database.Migrate();
		}

		// Configure the HTTP request pipeline.
		app.UseExceptionHandler();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseSwagger();
			app.UseSwaggerUI();

		}

		app.MapDefaultEndpoints();
		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseRouting();
		app.MapControllers();
		app.UseCors();

		app.Run();
	}
}



