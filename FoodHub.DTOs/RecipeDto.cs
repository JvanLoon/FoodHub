namespace FoodHub.DTOs;

public class RecipeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RecipeItemDto> Ingredients { get; set; } = new();
}
