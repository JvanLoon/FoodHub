namespace FoodHub.Persistence.Entities;
public class Ingredient(Guid id, string name)
{
	public Guid Id { get; set; } = id;
	public string Name { get; set; } = name;
}
