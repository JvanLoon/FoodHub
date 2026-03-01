using Microsoft.EntityFrameworkCore.Storage;
using FoodHub.Persistence.Repositories;

namespace FoodHub.Persistence.Persistence;

public class UnitOfWork(ApplicationDbContext context, RecipeRepository recipeRepository, IngredientRepository ingredientRepository, UserRepository userRepository, RoleRepository roleRepository)
{
	private IDbContextTransaction? _currentTransaction;

	public RecipeRepository RecipeRepository => recipeRepository;
	public IngredientRepository IngredientRepository => ingredientRepository;
	public UserRepository UserRepository => userRepository;
	public RoleRepository RoleRepository => roleRepository;

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
