using System.Text.Json.Serialization;

namespace FoodHub.Persistence.Entities;
public class ReceptIngredient : BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid(); // New surrogate key
	public Guid ReceptId { get; set; }
	[JsonIgnore]
	public virtual Recept Recept { get; set; }
	public Guid IngredientId { get; set; }

	public virtual Ingredient Ingredient { get; set; }
	public int Amount { get; set; }
	public IngredientAmountType IngredientAmount { get; set; }
}
