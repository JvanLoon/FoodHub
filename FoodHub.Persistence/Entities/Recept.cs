using System.Xml.Linq;

namespace FoodHub.Persistence.Entities;
public class Recept
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public List<Ingredient> Ingredients { get; set; } = [];

	public Recept(Guid id, string name, List<Ingredient> ingredients)
	{
		Id = id;
		Name = name;
		Ingredients = ingredients;
	}

	public Recept(Guid id, string name)
	{
		Id = id;
		Name = name;
	}
}
