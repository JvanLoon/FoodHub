using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodHub.Persistence.Configuration;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(ri => ri.ShouldBeAddedToShoppingCart)
            .HasDefaultValue(true);

        // Mirrors RecipeConfiguration: author lookups plus a partial index for the
        // review queue's "unreviewed only" scan.
        builder.Property(i => i.CreatedByUserId)
            .IsRequired();

        builder.HasIndex(i => i.CreatedByUserId);

        builder.HasIndex(i => i.IsReviewed)
            .HasDatabaseName("IX_Ingredients_IsReviewed_Pending")
            .HasFilter("\"IsReviewed\" = false");
    }
}