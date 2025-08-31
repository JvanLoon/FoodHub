using FoodCalc.Api.Extensions;
using FoodCalc.Features.Mapping;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories;
using FoodHub.Persistence.Repositories.Interface;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		var webBaseAddress = builder.Configuration["WebServer:BaseAddress"];

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

		// Add services to the container.
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

		// Add Identity
		builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
		{
			options.SignIn.RequireConfirmedAccount = false;
			options.User.RequireUniqueEmail = true;
		})
		.AddEntityFrameworkStores<ApplicationDbContext>()
		.AddDefaultTokenProviders();

		// Configure JWT authentication
		var jwtSettings = builder.Configuration.GetSection("Jwt");
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
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtSettings["Issuer"],
				IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"])),
				ClockSkew = TimeSpan.Zero
			};

			options.TokenValidationParameters.ValidAudience = "your-api-audience";
			options.TokenValidationParameters.ValidateAudience = true;

			options.SaveToken = true;
		});

		// Use the custom service registration method
		builder.Services.AddCustomServices();

		builder.Services.AddApplicationMediatR();

		// CORS config
		builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowWebApp",
				builder => builder
					.WithOrigins(webBaseAddress)
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials());
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

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseCors("AllowWebApp");
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
		app.MapDefaultEndpoints();

		app.Run();
	}
}



