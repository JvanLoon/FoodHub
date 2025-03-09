using System.Xml.Linq;

namespace FoodHub.Persistence.Entities;
public class Recept
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public List<Ingredient> Ingredients { get; set; } = [];
}
