using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FoodHub.Persistence.Entities;

namespace FoodHub.Persistence.Configuration;
public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
	public void Configure(EntityTypeBuilder<Ingredient> builder)
	{
		builder.HasKey(i => i.Id);
		builder.Property(i => i.Name).HasMaxLength(450).IsRequired();

		builder.Property(ri => ri.ShouldBeAddedToShoppingCart).HasDefaultValue(true);
	}
}
