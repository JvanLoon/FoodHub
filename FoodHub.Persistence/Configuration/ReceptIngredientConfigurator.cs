using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FoodHub.Persistence.Configuration;
public class ReceptIngredientConfigurator : IEntityTypeConfiguration<ReceptIngredient>
{
	public void Configure(EntityTypeBuilder<ReceptIngredient> builder)
	{
		builder.HasOne(ri => ri.Recept)
			.WithMany(r => r.ReceptIngredient)
			.HasForeignKey(ri => ri.ReceptId);

		builder.HasOne(ri => ri.Ingredient)
			.WithMany()
			.HasForeignKey(ri => ri.IngredientId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasKey(ri => new { ri.ReceptId, ri.IngredientId });

		builder.Property(ri => ri.Amount).IsRequired();
		builder.ToTable(t =>
			t.HasCheckConstraint("CK_ReceptIngredient_Amount", "Amount > 0"));

		builder.Property(ri => ri.IngredientAmount).IsRequired();
		//IngredientAmount may not be IngredientAmount.None
		builder.ToTable(t => 
			t.HasCheckConstraint("CK_ReceptIngredient_IngredientAmount", "IngredientAmount > 0"));
	}
}
