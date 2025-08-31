using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories.Interface;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.Persistence.Repositories
{
	public class UserRepository(ApplicationDbContext context) : IUserRepository
	{

		public Task<User> AddAsync(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<int> DeleteAsync(string id, CancellationToken cancellationToken)
		{
			context.Users.Remove(new User { Id = id });

			return context.SaveChangesAsync(cancellationToken);
		}

		public Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
		{
			return context.Users.ToListAsync(cancellationToken);
		}

		public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
		{
			return context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
		}

		public Task UpdateAsync(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
