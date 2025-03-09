using FoodHub.Persistence.Repositories.Interface;

namespace FoodHub.Persistence.Persistence;

public interface IUnitOfWork
{
    IReceptRepository ReceptRepository { get; }
    IIngredientRepository IngredientRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
