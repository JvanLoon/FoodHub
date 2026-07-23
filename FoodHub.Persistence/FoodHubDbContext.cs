using FoodHub.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class FoodHubDbContext : IdentityDbContext<IdentityUser>
{
	public FoodHubDbContext(DbContextOptions<FoodHubDbContext> options) : base(options) {}

	// Define your DbSets here
	public DbSet<Recipe> Recipes { get; set; }
	public DbSet<RecipeItem> RecipeItems { get; set; }
	public DbSet<Ingredient> Ingredients { get; set; }

	public DbSet<RecipeBlackList> RecipeBlackLists { get; set; }

	public DbSet<MealPlanEntry> MealPlanEntries { get; set; }

	public override int SaveChanges()
	{
		var entries = ChangeTracker.Entries<BaseEntity>();
		foreach (var entry in entries)
		{
			if (entry.State == EntityState.Modified) { entry.Entity.ModifiedDate = DateTime.UtcNow; }
		}

		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		var entries = ChangeTracker.Entries<BaseEntity>();
		foreach (var entry in entries)
		{
			if (entry.State == EntityState.Modified) { entry.Entity.ModifiedDate = DateTime.UtcNow; }
		}

		return base.SaveChangesAsync(cancellationToken);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Configure Identity + relational conventions first, then apply our
		// entity configurations and seed the default roles/users.
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodHubDbContext).Assembly);
		FoodHub.Persistence.Configuration.IdentitySeed.Seed(modelBuilder);
	}
}