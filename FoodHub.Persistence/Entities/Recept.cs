namespace FoodHub.Persistence.Entities;
public class Recept
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string? Name { get; set; } = string.Empty;
	public List<Ingredient>? Ingredients { get; set; } = [];

	public Recept()
	{
		Ingredients = new List<Ingredient>();
	}
}
