namespace FoodHub.Persistence.Entities;
public class Recept
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string? Name { get; set; }
	public List<Ingredient>? Ingredients { get; set; } = [];
}
