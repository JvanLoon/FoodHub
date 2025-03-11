namespace FoodHub.Persistence.Entities;
public class ReceptIngredient
{
	public Guid ReceptId { get; set; }
	public virtual Recept Recept { get; set; }
	public Guid IngredientId { get; set; }
	public virtual Ingredient Ingredient { get; set; }
	public int Amount { get; set; }
	public IngredientAmountType IngredientAmount { get; set; }
}
