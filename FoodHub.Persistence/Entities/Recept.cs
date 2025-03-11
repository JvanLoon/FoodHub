namespace FoodHub.Persistence.Entities;
public class Recept
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string? Name { get; set; } = string.Empty;
	public List<ReceptIngredient>? ReceptIngredient { get; set; } = [];

	public Recept()
	{
		ReceptIngredient = new List<ReceptIngredient>();
	}
}
