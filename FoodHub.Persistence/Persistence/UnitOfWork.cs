using FoodHub.Persistence.Repositories.Interface;

namespace FoodHub.Persistence.Persistence;

public class UnitOfWork(ApplicationDbContext context, IReceptRepository receptRepository, IIngredientRepository ingredientRepository) : IUnitOfWork
{
	public IReceptRepository ReceptRepository => receptRepository;

	public IIngredientRepository IngredientRepository => ingredientRepository;

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
