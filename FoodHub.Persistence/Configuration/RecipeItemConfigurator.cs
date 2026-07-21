using FoodHub.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FoodHub.Persistence.Configuration;

public class RecipeItemConfigurator : IEntityTypeConfiguration<RecipeItem>
{
	public void Configure(EntityTypeBuilder<RecipeItem> builder)
	{
		builder.HasKey(ri => ri.Id);

		builder.Property(ri => ri.Name)
			.HasMaxLength(450)
			.IsRequired();

		builder.Property(ri => ri.Amount)
			.IsRequired()
			.HasColumnType("decimal(10,2)");
		builder.ToTable(t =>
			t.HasCheckConstraint("CK_RecipeItem_Amount", "Amount > 0"));

		builder.Property(ri => ri.IngredientAmount).IsRequired();
		//IngredientAmount may not be IngredientAmount.None
		builder.ToTable(t =>
			t.HasCheckConstraint("CK_RecipeItem_IngredientAmount", "IngredientAmount > 0"));

		builder.Property(ri => ri.ShouldBeAddedToShoppingCart).HasDefaultValue(true);

		// Unique constraint: A recipe can only contain an ingredient (by name) once
		builder.HasIndex(ri => new { ri.RecipeId, ri.Name })
			.IsUnique()
			.HasDatabaseName("UX_RecipeItem_RecipeId_Name");
	}
}
