using FoodHub.DTOs;
using FoodHub.DTOs.Legacy;

namespace FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;

/// <summary>
/// Maps a legacy (pre-RecipeItem) export onto the current import shape. Each
/// legacy recipe-ingredient line is turned into a <see cref="RecipeItemDto"/>,
/// resolving the snapshot Name/flag from the line's nested Ingredient or, failing
/// that, from the exported ingredient catalog by IngredientId. Lines whose name
/// cannot be resolved are skipped (a RecipeItem cannot exist without a name).
/// </summary>
public static class LegacyImportConverter
{
	public static ImportExportAllDataDto ToCurrent(LegacyImportExportAllDataDto legacy)
	{
		var catalogById = legacy.Ingredients.GroupBy(i => i.Id)
								.ToDictionary(g => g.Key, g => g.First());

		var items = new List<RecipeItemDto>();
		foreach (var ri in legacy.RecipeIngredients)
		{
			catalogById.TryGetValue(ri.IngredientId, out var catalog);

			var name = ri.Ingredient?.Name ?? catalog?.Name;
			if (string.IsNullOrWhiteSpace(name)) { continue; }

			var shouldBeAddedToShoppingCart = ri.Ingredient?.ShouldBeAddedToShoppingCart
											  ?? catalog?.ShouldBeAddedToShoppingCart ?? true;

			items.Add(new RecipeItemDto
			{
				Id = ri.Id,
				RecipeId = ri.RecipeId,
				Name = name,
				Amount = ri.Amount,
				IngredientAmount = ri.IngredientAmount,
				ShouldBeAddedToShoppingCart = shouldBeAddedToShoppingCart
			});
		}

		return new ImportExportAllDataDto
		{
			Recipes = legacy.Recipes,
			Ingredients = legacy.Ingredients,
			RecipeItems = items,
			Users = legacy.Users
		};
	}
}