namespace FoodHub.Persistence.Entities;
public class Ingredient(Guid id, string name, string amount, int prio = 0)
{
	public Guid Id { get; set; } = id;
	public string Name { get; set; } = name;
	public string Amount { get; set; } = amount;
	public int Prio { get; set; } = prio;
}
