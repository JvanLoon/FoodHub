namespace FoodHub.Persistence.Entities;

public class Recipe : BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string? Name { get; set; } = string.Empty;
	public List<RecipeItem> Ingredients { get; set; } = new();
}
