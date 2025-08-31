using FoodHub.Persistence.Repositories.Interface;

namespace FoodHub.Persistence.Persistence;

public interface IUnitOfWork
{
    IRecipeRepository RecipeRepository { get; }
    IIngredientRepository IngredientRepository { get; }
	IUserRepository UserRepository { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
