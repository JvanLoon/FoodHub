using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FoodHub.Persistence.Configuration;
public class IngredientAmountTypeConfigurator : IEntityTypeConfiguration<IngredientAmountTypeEntity>
{
	public void Configure(EntityTypeBuilder<IngredientAmountTypeEntity> builder)
	{
		builder.ToTable("IngredientAmountTypes");

		// Set primary key
		builder.HasKey(e => e.Id);

		builder.Property(e => e.Id)
			.ValueGeneratedNever(); // Prevents auto-increment, uses enum value

		builder.Property(e => e.Name)
			.IsRequired()
			.HasMaxLength(50);

		// Seed data from enum
		builder.HasData(
			Enum.GetValues(typeof(IngredientAmountType))
				.Cast<IngredientAmountType>()
				.Select(e => new IngredientAmountTypeEntity { Id = (int) e, Name = e.ToString() })
		);
	}
}

public class IngredientAmountTypeEntity
{
	public required int Id { get; set; }
	public required string Name { get; set; }
}
