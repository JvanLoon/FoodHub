using FoodHub.Persistence.Entities;

namespace FoodHub.Persistence.Repositories.Interface;
public interface IRecipeRepository
{
    Task<List<Recipe>> GetAllAsync(CancellationToken cancellationToken);
    Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Recipe> AddAsync(Recipe recipe, CancellationToken cancellationToken);
    Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);

	Task AddRecipeIngredientAsync(RecipeIngredient recipeIngredient, CancellationToken cancellationToken);
	Task DeleteRecipeIngredientAsync(Guid recipeIngredientId, CancellationToken cancellationToken);
}
