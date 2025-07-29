using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;

public class AggregatedIngredientService
{
	public List<RecipeIngredientDto> AggregatedIngredients { get; set; } = new();
}
