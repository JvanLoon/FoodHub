using System.Text.Json.Serialization;

namespace FoodHub.Persistence.Entities;
public class RecipeIngredient : BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid(); // New surrogate key
	public Guid RecipeId { get; set; }
	[JsonIgnore]
	public virtual Recipe Recipe { get; set; } = null!;
	public Guid IngredientId { get; set; }
	public virtual Ingredient Ingredient { get; set; } = null!;
	public decimal Amount { get; set; }
	public IngredientAmountType IngredientAmount { get; set; }
}
