using FoodHub.Persistence.Persistence;
using FoodHub.Persistence.Repositories;

namespace FoodCalc.Api.Extensions
{
	public static class Extentions
	{
		public static IServiceCollection AddCustomServices(this IServiceCollection services)
		{
			services.AddScoped<RecipeRepository>();
			services.AddScoped<IngredientRepository>();
			services.AddScoped<UserRepository>();
			services.AddScoped<RoleRepository>();

			services.AddScoped<UnitOfWork>();

			return services;
		}
	}
}
