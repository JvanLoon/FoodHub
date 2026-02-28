using FoodHub.Persistence.Entities;

namespace FoodHub.Persistence.Repositories.Interface;
public interface IIngredientRepository
{
    Task<List<Ingredient>> GetAllAsync(CancellationToken cancellationToken);
    Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Ingredient?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken);
    Task UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
