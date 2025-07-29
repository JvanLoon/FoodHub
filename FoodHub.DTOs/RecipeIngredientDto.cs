namespace FoodHub.DTOs;

public class RecipeIngredientDto
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public IngredientDto Ingredient { get; set; } = new();
    public decimal Amount { get; set; }
    public IngredientAmountTypeDto IngredientAmount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
