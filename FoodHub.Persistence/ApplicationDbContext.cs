using FoodHub.Persistence.Entities;
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
	public DbSet<User> Users { get; set; }

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
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(login => new { login.LoginProvider, login.ProviderKey });
	}
}
