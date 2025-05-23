using FoodHub.Persistence.Entities;

namespace FoodCalc.Web.Components.Services;

public class AggregatedIngredientService
{
	public List<RecipeIngredient> AggregatedIngredients { get; set; } = new();
}
