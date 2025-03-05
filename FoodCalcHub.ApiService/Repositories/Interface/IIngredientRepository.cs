using FoodCalcHub.ApiService.Entities;

namespace FoodCalcHub.ApiService.Repositories.Interface;
public interface IIngredientRepository
{
    Task<IEnumerable<Ingredient>> GetAllAsync(CancellationToken cancellationToken);
    Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken);
    Task UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}