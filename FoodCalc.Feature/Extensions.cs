using FoodCalc.Features.Recepts.Commands.AddIngredientToRecept;
using FoodCalc.Features.Recepts.Commands.AddRecept;
using FoodCalc.Features.Recepts.Commands.DeleteRecept;
using FoodCalc.Features.Recepts.Commands.UpdateRecept;
using FoodCalc.Features.Recepts.Queries.GetAllRecepts;
using FoodCalc.Features.Recepts.Queries.GetById;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using FoodHub.Persistence.Repositories;
using FoodHub.Persistence.Repositories.Interface;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
	//public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	//{
	//	// Uncomment the following to restrict the allowed schemes for service discovery.
	//	// builder.Services.Configure<ServiceDiscoveryOptions>(options =>
	//	// {
	//	//     options.AllowedSchemes = ["https"];
	//	// });

	//	return builder;
	//}

	public static IServiceCollection AddCustomServices(this IServiceCollection services)
	{
		services.AddTransient<IReceptRepository, ReceptRepository>();
		services.AddTransient<IIngredientRepository, IngredientRepository>();

		services.AddTransient<IUnitOfWork, UnitOfWork>();

		return services;
	}

	public static IServiceCollection AddApplicationMediatR(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
		{
			//queries
			//cfg.RegisterServicesFromAssembly(typeof(GetAllReceptsQueryHandler).Assembly);
			//cfg.RegisterServicesFromAssembly(typeof(GetReceptByIdQueryHandler).Assembly);

			cfg.RegisterServicesFromAssemblyContaining<GetAllReceptsQueryHandler>();
			cfg.RegisterServicesFromAssemblyContaining<GetReceptByIdQueryHandler>();

			//commands
			//cfg.RegisterServicesFromAssembly(typeof(AddReceptCommandHandler).Assembly);
			//cfg.RegisterServicesFromAssembly(typeof(DeleteReceptCommandHandler).Assembly);
			//cfg.RegisterServicesFromAssembly(typeof(UpdateReceptCommandHandler).Assembly);
			//cfg.RegisterServicesFromAssembly(typeof(AddIngredientToReceptCommandHandler).Assembly);

			cfg.RegisterServicesFromAssemblyContaining<AddReceptCommandHandler>();
			cfg.RegisterServicesFromAssemblyContaining<DeleteReceptCommandHandler>();
			cfg.RegisterServicesFromAssemblyContaining<UpdateReceptCommandHandler>();
			cfg.RegisterServicesFromAssemblyContaining<AddIngredientToReceptCommandHandler>();
		});

		//services.AddTransient<IRequestHandler<GetReceptByIdQuery, Recept>, GetReceptByIdQueryHandler>();

		return services;
	}
}
