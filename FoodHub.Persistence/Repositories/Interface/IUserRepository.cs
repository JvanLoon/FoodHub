using Microsoft.AspNetCore.Identity;

namespace FoodHub.Persistence.Repositories.Interface;
public interface IUserRepository
{
	Task<List<IdentityUser>> GetAllAsync(CancellationToken cancellationToken);
	Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);
	Task<IdentityUser> AddAsync(IdentityUser IdentityUser, CancellationToken cancellationToken);
	Task UpdateAsync(IdentityUser IdentityUser, CancellationToken cancellationToken);
	Task<int> DeleteAsync(string id, CancellationToken cancellationToken);
	Task AddRoleToUser(IdentityUser IdentityUser, string role, CancellationToken cancellationToken);
}
