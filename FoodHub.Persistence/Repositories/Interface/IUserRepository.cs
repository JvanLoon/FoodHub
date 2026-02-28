using Microsoft.AspNetCore.Identity;

namespace FoodHub.Persistence.Repositories.Interface;
public interface IUserRepository
{
	Task<List<IdentityUser>> GetAllAsync(CancellationToken cancellationToken);
	Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);
	Task UpdateAsync(IdentityUser IdentityUser, CancellationToken cancellationToken);
	Task AddRoleToUser(IdentityUser IdentityUser, string role, CancellationToken cancellationToken);
	Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken);
	Task RemoveRoleFromUser(IdentityUser user, string role, CancellationToken cancellationToken);
	Task AddRecipeToBlackList(Guid userId, Guid recipeId);
	Task RemoveRecipeToBlackList(Guid userId, Guid recipeId);
}
