using FoodHub.Persistence.Repositories.Interface;

namespace FoodHub.Persistence.Persistence;

public class UnitOfWork(ApplicationDbContext context, IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository, IUserRepository userRepository) : IUnitOfWork
{
	public IRecipeRepository RecipeRepository => recipeRepository;

	public IIngredientRepository IngredientRepository => ingredientRepository;

	public IUserRepository UserRepository => userRepository;

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
