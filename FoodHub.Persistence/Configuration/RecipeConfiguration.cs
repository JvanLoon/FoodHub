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

		builder.HasMany(r => r.Ingredients)
			   .WithOne()
			   .HasForeignKey(k => k.RecipeId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.Navigation(r => r.Ingredients)
			   .AutoInclude();
	}
}