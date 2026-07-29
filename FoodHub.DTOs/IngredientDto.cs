namespace FoodHub.DTOs;

public class IngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool ShouldBeAddedToShoppingCart { get; set; } = true;
    public string CreatedByUserId { get; set; } = string.Empty;

    /// <summary>False while the catalog entry is awaiting moderator approval.</summary>
    public bool IsReviewed { get; set; }
}