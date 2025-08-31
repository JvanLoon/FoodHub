using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories.Interface;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.Persistence.Repositories
{
	public class UserRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager) : IUserRepository
	{

		public Task<IdentityUser> AddAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<int> DeleteAsync(string id, CancellationToken cancellationToken)
		{
			context.Users.Remove(new IdentityUser { Id = id });

			return context.SaveChangesAsync(cancellationToken);
		}

		public Task<List<IdentityUser>> GetAllAsync(CancellationToken cancellationToken)
		{
			return userManager.Users.ToListAsync(cancellationToken);
		}

		public Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
		{
			return context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
		}

		public Task UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task AddRoleToUser(IdentityUser user, string role, CancellationToken cancellationToken)
		{
			return userManager.AddToRoleAsync(user, role);
		}
	}
}
