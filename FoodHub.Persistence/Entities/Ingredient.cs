namespace FoodHub.Persistence.Entities;
public class Ingredient
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Amount { get; set; }
    public int Prio { get; set; } = 0;
}
