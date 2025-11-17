using FoodHub.Persistence.Persistence;
using FoodHub.Persistence.Repositories;
using FoodHub.Persistence.Repositories.Interface;

namespace FoodCalc.Api.Extensions
{
	public static class Extentions
	{
		public static IServiceCollection AddCustomServices(this IServiceCollection services)
		{
			services.AddScoped<IRecipeRepository, RecipeRepository>();
			services.AddScoped<IIngredientRepository, IngredientRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IRoleRepository, RoleRepository>();

			services.AddScoped<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}
