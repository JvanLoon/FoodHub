using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FoodHub.Persistence.Entities;

namespace FoodHub.Persistence.Configuration;
public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
	public void Configure(EntityTypeBuilder<Recipe> builder)
	{
		builder.HasKey(r => r.Id);
		builder.HasIndex(r => r.Name).IsUnique();
		builder.Property(r => r.Name).HasMaxLength(450).IsRequired();
		builder.Navigation(r => r.RecipeIngredient).AutoInclude();

		builder.HasMany(r => r.RecipeIngredient)
			.WithOne()
			.HasForeignKey(k => k.RecipeId)
			.OnDelete(DeleteBehavior.Cascade);

		//also include Recipe.RecipeIngredient.Ingredient

	}
}
