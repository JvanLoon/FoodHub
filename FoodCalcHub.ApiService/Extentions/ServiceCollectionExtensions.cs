using FoodCalcHub.ApiService.Repositories.Interface;
using FoodCalcHub.ApiService.Repositories;
using System.Reflection;
using MediatR;
using FoodCalcHub.ApiService.Persistence;

namespace FoodCalcHub.ApiService.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped<IReceptRepository, ReceptRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();

        // Register IUnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>(); // Add this line

        return services;
    }
}
