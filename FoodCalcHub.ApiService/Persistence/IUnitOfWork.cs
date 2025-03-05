using FoodCalcHub.ApiService.Repositories.Interface;

namespace FoodCalcHub.ApiService.Persistence;

public interface IUnitOfWork
{
    IReceptRepository ReceptRepository { get; }
    IIngredientRepository IngredientRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}