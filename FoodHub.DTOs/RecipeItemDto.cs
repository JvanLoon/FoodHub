namespace FoodHub.DTOs;

public class RecipeItemDto
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public required Guid IngredientId { get; set; }
    public required IngredientDto Ingredient { get; set; }
    public decimal Amount { get; set; }
    public IngredientAmountTypeDto IngredientAmount { get; set; }
    public bool ShouldBeAddedToShoppingCart { get; set; } = true;
}