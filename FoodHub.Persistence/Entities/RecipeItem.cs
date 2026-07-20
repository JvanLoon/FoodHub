namespace FoodHub.Persistence.Entities;

/// <summary>
/// An ingredient line that belongs to a single <see cref="Recipe"/>. This is no
/// longer a link table to <see cref="Ingredient"/>: the name (and the
/// shopping-cart flag) are snapshotted onto the line, so a recipe exposes its
/// ingredients directly as recipe.Ingredients[i].Name. The <see cref="Ingredient"/>
/// entity remains as a separate catalog used for autocomplete/management.
/// </summary>
public class RecipeItem : BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid RecipeId { get; set; }
	public required string Name { get; set; }
	public decimal Amount { get; set; }
	public IngredientAmountType IngredientAmount { get; set; }
	public bool ShouldBeAddedToShoppingCart { get; set; } = true;
}
