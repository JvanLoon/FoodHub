namespace FoodHub.DTOs;

public class RecipeItemDto
{
    public Guid Id { get; set; } = new Guid();
    public Guid RecipeId { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The catalog ingredient this line refers to. Clients compare lines on this, never on
    /// <see cref="Name"/>. A client may send it, but the server resolves it from the name
    /// anyway, so it is only a hint on the way in and authoritative on the way out.
    /// </summary>
    public Guid? IngredientId { get; set; }

    public decimal Amount { get; set; }
    public IngredientAmountTypeDto IngredientAmount { get; set; }
    public bool ShouldBeAddedToShoppingCart { get; set; } = true;
}