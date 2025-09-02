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
		public Task<List<IdentityUser>> GetAllAsync(CancellationToken cancellationToken)
		{
			return userManager.Users.ToListAsync(cancellationToken);
		}

		public async Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
		{
			IdentityUser? user = await context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
			if(user != null)
			{
				throw new Exception("User not found");
			}
			if (!user!.LockoutEnabled)
			{
				throw new Exception("User locked");
			}
			if (!user!.EmailConfirmed)
			{
				throw new Exception("User not enabled");
			}
			return user;
		}

		public async Task UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task AddRoleToUser(IdentityUser user, string role, CancellationToken cancellationToken)
		{
			return userManager.AddToRoleAsync(user, role);
		}

        public Task AddRecipeToBlackList(Guid userId, Guid recipeId)
        {
            context.RecipeBlackLists.Add(new RecipeBlackList { RecipeId = recipeId, UserId = userId });
            return context.SaveChangesAsync();
        }

		public Task RemoveRecipeToBlackList(Guid userId, Guid recipeId)
		{
			context.RecipeBlackLists.Remove(new RecipeBlackList { RecipeId = recipeId, UserId = userId });
			return context.SaveChangesAsync();
		}
	}
}
