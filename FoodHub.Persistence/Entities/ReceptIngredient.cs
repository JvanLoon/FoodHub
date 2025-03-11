using System.Text.Json.Serialization;

namespace FoodHub.Persistence.Entities;
public class ReceptIngredient
{
	public Guid ReceptId { get; set; }
	[JsonIgnore]
	public virtual Recept Recept { get; set; }
	public Guid IngredientId { get; set; }
	[JsonIgnore]
	public virtual Ingredient Ingredient { get; set; }
	public int Amount { get; set; }
	public IngredientAmountType IngredientAmount { get; set; }
}
