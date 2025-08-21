using FoodCalc.Api.Extensions;
using FoodCalc.Features.Mapping;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

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

		// Use the custom service registration method
		builder.Services.AddCustomServices();

		builder.Services.AddApplicationMediatR();

		var app = builder.Build();

		// Run migrations at startup
		using (var scope = app.Services.CreateScope())
		{
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			db.Database.Migrate();
		}

		// Apply migrations at startup
		//using (var scope = app.Services.CreateScope())
		//{
		//	var services = scope.ServiceProvider;
		//	try
		//	{
		//		var dbContext = services.GetRequiredService<ApplicationDbContext>();
		//		// Add retries to handle SQL Server startup timing
		//		var retryCount = 0;
		//		const int maxRetries = 5;

		//		while (retryCount < maxRetries)
		//		{
		//			try
		//			{
		//				// Apply pending migrations
		//				dbContext.Database.Migrate();

		//				// Log success
		//				var logger = services.GetRequiredService<ILogger<Program>>();
		//				logger.LogInformation("Database migrations applied successfully");

		//				// Break out of retry loop on success
		//				break;
		//			}
		//			catch (Exception ex) when (retryCount < maxRetries - 1)
		//			{
		//				// Log the error but continue retrying
		//				var logger = services.GetRequiredService<ILogger<Program>>();
		//				logger.LogWarning(ex, "Error applying migrations (attempt {RetryCount}/{MaxRetries}). Retrying in 5 seconds...",
		//					retryCount + 1, maxRetries);

		//				retryCount++;
		//				Thread.Sleep(5000); // Wait 5 seconds before retrying
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log fatal error if all retries failed
		//		var logger = services.GetRequiredService<ILogger<Program>>();
		//		logger.LogError(ex, "Fatal error occurred while applying database migrations");

		//		// Optionally, you could rethrow to prevent the application from starting
		//		// throw;
		//	}
		//}

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

		app.Run();
	}
}



