using FoodHub.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
public class ApplicationDbContext : IdentityDbContext<User>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

	// Define your DbSets here
	public DbSet<Recipe> Recipes { get; set; }
	public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
	public DbSet<Ingredient> Ingredients { get; set; }

	public override int SaveChanges()
	{
		var entries = ChangeTracker.Entries<BaseEntity>();
		foreach (var entry in entries)
		{
			if (entry.State == EntityState.Modified)
			{
				entry.Entity.ModifiedDate = DateTime.UtcNow;
			}
		}
		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		var entries = ChangeTracker.Entries<BaseEntity>();
		foreach (var entry in entries)
		{
			if (entry.State == EntityState.Modified)
			{
				entry.Entity.ModifiedDate = DateTime.UtcNow;
			}
		}
		return base.SaveChangesAsync(cancellationToken);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
		// Rename Identity tables
		modelBuilder.Entity<User>().ToTable("Users");
		modelBuilder.Entity<IdentityRole>().ToTable("Role");
		modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("Claims");
		modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
		modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
		modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
		modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
		
		base.OnModelCreating(modelBuilder);
	}
}
