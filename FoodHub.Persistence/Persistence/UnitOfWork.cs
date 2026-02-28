using Microsoft.EntityFrameworkCore.Storage;
using FoodHub.Persistence.Repositories.Interface;

namespace FoodHub.Persistence.Persistence;

public class UnitOfWork(ApplicationDbContext context, IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository, IUserRepository userRepository, IRoleRepository roleRepository) : IUnitOfWork
{
	private IDbContextTransaction? _currentTransaction;

	public IRecipeRepository RecipeRepository => recipeRepository;
	public IIngredientRepository IngredientRepository => ingredientRepository;
	public IUserRepository UserRepository => userRepository;
	public IRoleRepository RoleRepository => roleRepository;

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            return _currentTransaction;
        _currentTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}
