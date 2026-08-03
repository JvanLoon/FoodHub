namespace FoodHub.DTOs;

public class RecipeItemDto
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public IngredientDto Ingredient { get; set; } = null!;
    public decimal Amount { get; set; }
    public IngredientAmountTypeDto IngredientAmount { get; set; }
    public bool ShouldBeAddedToShoppingCart { get; set; } = true;
}