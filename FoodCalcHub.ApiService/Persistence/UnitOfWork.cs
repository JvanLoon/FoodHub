using FoodCalcHub.ApiService.Repositories.Interface;

namespace FoodCalcHub.ApiService.Persistence;

public class UnitOfWork(ApplicationDbContext context, IReceptRepository receptRepository, IIngredientRepository ingredientRepository) : IUnitOfWork
{
    public IReceptRepository ReceptRepository { get; }
    public IIngredientRepository IngredientRepository { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}