using FoodHub.Persistence.Entities;
using FoodHub.ServiceDefaults;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Persistence.Repositories
{
	public class UserRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager)
	{
		public IQueryable<IdentityUser> GetAllAsync()
		{
			return context.Users.AsQueryable();
		}

		public async Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
		{
			IdentityUser? user = await context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
			if (user != null)
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
			// Find the existing user by Id
			var existingUser = await context.Users.FindAsync([user.Id], cancellationToken);
			if (existingUser == null)
				throw new InvalidOperationException($"User with Id {user.Id} not found.");

			// Update properties as needed
			existingUser.Email = user.Email;
			existingUser.UserName = user.UserName;
			existingUser.EmailConfirmed = user.EmailConfirmed;
			existingUser.LockoutEnabled = user.LockoutEnabled;

			//security
			existingUser.PasswordHash = user.PasswordHash;
			existingUser.SecurityStamp = user.SecurityStamp;
			existingUser.ConcurrencyStamp = user.ConcurrencyStamp;

			//normalized strings
			existingUser.NormalizedEmail = user.Email!.NormalizeToUpper();
			existingUser.NormalizedUserName = user.UserName!.NormalizeToUpper();

			//todo: adjust according to your security policies
			if (true)
			{
				existingUser.AccessFailedCount = 0;
				existingUser.LockoutEnd = null;
			}

			//nullable
			existingUser.PhoneNumber = null;
			existingUser.PhoneNumberConfirmed = false;
			existingUser.TwoFactorEnabled = false;

			// Use UserManager to update (handles hashing, etc.)
			var result = await userManager.UpdateAsync(existingUser);
			if (!result.Succeeded)
				throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

			await context.SaveChangesAsync(cancellationToken);
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
		public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			return await userManager.GetRolesAsync(user);
		}

		public async Task RemoveRoleFromUser(IdentityUser user, string role, CancellationToken cancellationToken)
		{
			await userManager.RemoveFromRoleAsync(user, role);
		}
	}
}
