namespace FoodHub.DTOs;

public class RecipeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// False while the recipe is awaiting moderator approval. A client only ever receives an
    /// unapproved recipe if the caller is its author, so this reads as "mine, not yet public".
    /// </summary>
    public bool IsReviewed { get; set; }

    public List<RecipeItemDto> Ingredients { get; set; } = [];
}