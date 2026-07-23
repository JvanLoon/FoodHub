namespace FoodHub.DTOs;

public class ImportExportAllDataDto
{
	public List<RecipeDto> Recipes { get; set; } = [];
	public List<IngredientDto> Ingredients { get; set; } = [];
	public List<RecipeItemDto> RecipeItems { get; set; } = [];
	public List<UserWithRolesDto>? Users { get; set; }
}