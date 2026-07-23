using FoodHub.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodHub.Persistence.Configuration;

public class MealPlanEntryConfiguration : IEntityTypeConfiguration<MealPlanEntry>
{
	public void Configure(EntityTypeBuilder<MealPlanEntry> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.UserId)
			.IsRequired();

		builder.Property(x => x.Date)
			.IsRequired();

		// Fast lookups for "give me this user's plan for a date range".
		builder.HasIndex(x => new { x.UserId, x.Date });

		builder.HasOne(x => x.Recipe)
			.WithMany()
			.HasForeignKey(x => x.RecipeId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.ToTable("MealPlanEntries");
	}
}
