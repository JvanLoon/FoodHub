using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FoodHub.Persistence.Entities;

namespace FoodHub.Persistence.Configuration;
public class ReceptConfiguration : IEntityTypeConfiguration<Recept>
{
	public void Configure(EntityTypeBuilder<Recept> builder)
	{
		builder.HasKey(r => r.Id);
		builder.HasIndex(r => r.Name).IsUnique();
		builder.Property(r => r.Name).IsRequired();

		builder.HasMany(r => r.ReceptIngredient)
			.WithOne()
			.OnDelete(DeleteBehavior.Cascade);
	}
}
