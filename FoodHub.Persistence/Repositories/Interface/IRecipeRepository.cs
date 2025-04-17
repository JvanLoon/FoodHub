using FoodHub.Persistence.Entities;

namespace FoodHub.Persistence.Repositories.Interface;
public interface IRecipeRepository
{
    Task<List<Recipe>> GetAllAsync(CancellationToken cancellationToken);
    Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Recipe> AddAsync(Recipe recept, CancellationToken cancellationToken);
    Task UpdateAsync(Recipe recept, CancellationToken cancellationToken);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);

	Task AddReceptIngredientAsync(RecipeIngredient receptIngredient, CancellationToken cancellationToken);
	Task DeleteReceptIngredientAsync(Guid receptIngredientId, CancellationToken cancellationToken);
}
