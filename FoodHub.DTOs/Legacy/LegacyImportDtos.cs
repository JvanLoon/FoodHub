namespace FoodHub.DTOs.Legacy;

/// <summary>
/// The pre-RecipeItem export shape: RecipeIngredient was a link table with an
/// <see cref="LegacyRecipeIngredientDto.IngredientId"/> (and an optional nested
/// <see cref="LegacyRecipeIngredientDto.Ingredient"/>). Kept only so that JSON
/// exports produced before the RecipeItem rework can still be imported. New
/// exports use <see cref="ImportExportAllDataDto"/>.
/// </summary>
public class LegacyImportExportAllDataDto
{
	public List<RecipeDto> Recipes { get; set; } = [];
	public List<IngredientDto> Ingredients { get; set; } = [];
	public List<LegacyRecipeIngredientDto> RecipeIngredients { get; set; } = [];
	public List<UserWithRolesDto>? Users { get; set; }
}

public class LegacyRecipeIngredientDto
{
	public Guid Id { get; set; }
	public Guid RecipeId { get; set; }
	public Guid IngredientId { get; set; }
	public IngredientDto? Ingredient { get; set; }
	public decimal Amount { get; set; }
	public IngredientAmountTypeDto IngredientAmount { get; set; }
}
