namespace FoodHub.DTOs;

public class RecipeIngredientDto
{
	public Guid Id { get; set; } = new Guid();
	public Guid RecipeId { get; set; }
	public Guid IngredientId { get; set; }
	public IngredientDto? Ingredient { get; set; }
	public decimal Amount { get; set; }
	public IngredientAmountTypeDto IngredientAmount { get; set; }
}
