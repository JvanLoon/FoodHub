namespace FoodHub.DTOs;

public class RecipeItemDto
{
	public Guid Id { get; set; } = new Guid();
	public Guid RecipeId { get; set; }
	public string Name { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public IngredientAmountTypeDto IngredientAmount { get; set; }
	public bool ShouldBeAddedToShoppingCart { get; set; } = true;
}
