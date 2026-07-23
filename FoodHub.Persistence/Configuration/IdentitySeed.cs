using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Persistence.Configuration;

/// <summary>
/// Seeds the Admin/Moderator/User roles and two default accounts so a fresh
/// database has a working login out of the box:
///   • admin@foodhub.local  (Admin + Moderator + User roles)
///   • user@foodhub.local   (User role)
///
/// Passwords are the defaults documented in the README — change them after the
/// first login. All values are static (fixed ids/stamps/hashes) because EF Core
/// <c>HasData</c> requires deterministic seed data.
/// </summary>
public static class IdentitySeed
{
	// Default credentials (see README). CHANGE THESE after first login.
	public const string AdminEmail = "admin@foodhub.local";
	public const string UserEmail = "user@foodhub.local";

	private const string AdminRoleId = "b1e9a1a0-0000-0000-0000-000000000001";
	private const string ModeratorRoleId = "b1e9a1a0-0000-0000-0000-000000000002";
	private const string UserRoleId = "b1e9a1a0-0000-0000-0000-000000000003";

	private const string AdminUserId = "c2f0b2b0-0000-0000-0000-000000000001";
	private const string UserUserId = "c2f0b2b0-0000-0000-0000-000000000002";

	// Pre-computed with PasswordHasher<IdentityUser> (ASP.NET Core Identity v3).
	// admin => "Admin123!", user => "User123!".
	private const string AdminPasswordHash =
		"AQAAAAIAAYagAAAAEI9e1iMqykqith/CN4pGUK+H1zrVfp5ERttCqYZzKAMBWKvik5W8jJt1ukNyZIw+6g==";

	private const string UserPasswordHash =
		"AQAAAAIAAYagAAAAEMQmfpGbK2nWDteIHDEYhwcLUe0UDntdRwKgvrKQ5Jatcian7eTgcM4yD6Q10kKcHg==";

	public static void Seed(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<IdentityRole>()
					.HasData(new IdentityRole
					{
						Id = AdminRoleId,
						Name = "Admin",
						NormalizedName = "ADMIN",
						ConcurrencyStamp = AdminRoleId
					}, new IdentityRole
					{
						Id = ModeratorRoleId,
						Name = "Moderator",
						NormalizedName = "MODERATOR",
						ConcurrencyStamp = ModeratorRoleId
					}, new IdentityRole
					{
						Id = UserRoleId,
						Name = "User",
						NormalizedName = "USER",
						ConcurrencyStamp = UserRoleId
					});

		modelBuilder.Entity<IdentityUser>()
					.HasData(new IdentityUser
					{
						Id = AdminUserId,
						UserName = AdminEmail,
						NormalizedUserName = AdminEmail.ToUpperInvariant(),
						Email = AdminEmail,
						NormalizedEmail = AdminEmail.ToUpperInvariant(),
						EmailConfirmed = true,
						PasswordHash = AdminPasswordHash,
						SecurityStamp = AdminUserId,
						ConcurrencyStamp = AdminUserId,
						LockoutEnabled = false,
					}, new IdentityUser
					{
						Id = UserUserId,
						UserName = UserEmail,
						NormalizedUserName = UserEmail.ToUpperInvariant(),
						Email = UserEmail,
						NormalizedEmail = UserEmail.ToUpperInvariant(),
						EmailConfirmed = true,
						PasswordHash = UserPasswordHash,
						SecurityStamp = UserUserId,
						ConcurrencyStamp = UserUserId,
						LockoutEnabled = false,
					});

		modelBuilder.Entity<IdentityUserRole<string>>()
					.HasData(new IdentityUserRole<string>
					{
						UserId = AdminUserId,
						RoleId = AdminRoleId
					}, new IdentityUserRole<string>
					{
						UserId = AdminUserId,
						RoleId = ModeratorRoleId
					}, new IdentityUserRole<string>
					{
						UserId = AdminUserId,
						RoleId = UserRoleId
					}, new IdentityUserRole<string>
					{
						UserId = UserUserId,
						RoleId = UserRoleId
					});
	}
}