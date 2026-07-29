using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodHub.Persistence.Configuration;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.Property(r => r.Name)
            .HasMaxLength(450)
            .IsRequired();

        // Required author (legacy rows backfilled to the first admin by migration);
        // indexed for "recipes by user" lookups.
        builder.Property(r => r.CreatedByUserId)
            .IsRequired();

        builder.HasIndex(r => r.CreatedByUserId);

        // Partial index: the review queue asks for the unreviewed rows, which are a small
        // minority once a library is established. Quoted for the same reason as the check
        // constraints in RecipeItemConfigurator — PostgreSQL folds unquoted identifiers.
        builder.HasIndex(r => r.IsReviewed)
            .HasDatabaseName("IX_Recipes_IsReviewed_Pending")
            .HasFilter("\"IsReviewed\" = false");

        builder.HasMany(r => r.Ingredients)
            .WithOne()
            .HasForeignKey(k => k.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(r => r.Ingredients)
            .AutoInclude();
    }
}