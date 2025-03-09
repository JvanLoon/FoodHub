using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Define your DbSets here
    public DbSet<Recept> Recepts { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    // Define your model configuration here
    //    modelBuilder.Entity<Recept>()
    //        .HasMany(r => r.Ingredients)
    //        .WithOne(i => i.Recept)
    //        .HasForeignKey(i => i.ReceptId);
    //}
}
