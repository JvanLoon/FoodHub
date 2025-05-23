using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FoodHub.Persistence.Configuration;
public class RecipeIngredientConfigurator : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Amount).IsRequired();
        builder.ToTable(t =>
            t.HasCheckConstraint("CK_RecipeIngredient_Amount", "Amount > 0"));

        builder.Property(ri => ri.IngredientAmount).IsRequired();
        //IngredientAmount may not be IngredientAmount.None
        builder.ToTable(t =>
            t.HasCheckConstraint("CK_RecipeIngredient_IngredientAmount", "IngredientAmount > 0"));

        // Unique constraint: A recipe can only contain an ingredient once
        builder.HasIndex(ri => new { ri.RecipeId, ri.IngredientId })
            .IsUnique()
            .HasDatabaseName("UX_RecipeIngredient_RecipeId_IngredientId");

        builder.Navigation(n => n.Ingredient).AutoInclude();
        builder.Navigation(n => n.Recipe).AutoInclude();

        builder.HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredient)
            .HasForeignKey(ri => ri.RecipeId);

        builder.HasOne(ri => ri.Ingredient)
            .WithMany()
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);
	}
}
