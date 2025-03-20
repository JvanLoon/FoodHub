namespace FoodHub.Persistence.Entities;
public class Recept : BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string? Name { get; set; } = string.Empty;
	public List<ReceptIngredient>? ReceptIngredient { get; set; } = new();
}
