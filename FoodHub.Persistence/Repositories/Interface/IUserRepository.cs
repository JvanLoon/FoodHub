using FoodHub.Persistence.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.Persistence.Repositories.Interface;
public interface IUserRepository
{
	Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
	Task<User> AddAsync(User user, CancellationToken cancellationToken);
	Task UpdateAsync(User user, CancellationToken cancellationToken);
	Task<int> DeleteAsync(string id, CancellationToken cancellationToken);
}
