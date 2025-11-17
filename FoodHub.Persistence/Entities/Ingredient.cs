namespace FoodHub.Persistence.Entities;
public class Ingredient : BaseEntity
{
	public Guid Id { get; set; }
	public required string Name { get; set; }

	public bool ShouldBeAddedToShoppingCart { get; set; } = true;
}
