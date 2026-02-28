using Microsoft.EntityFrameworkCore.Storage;
using FoodHub.Persistence.Repositories.Interface;

namespace FoodHub.Persistence.Persistence;

public interface IUnitOfWork
{
    IRecipeRepository RecipeRepository { get; }
    IIngredientRepository IngredientRepository { get; }
	IUserRepository UserRepository { get; }
	IRoleRepository RoleRepository { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
