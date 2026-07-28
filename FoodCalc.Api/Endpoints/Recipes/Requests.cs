using FastEndpoints;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>
/// Query parameters for GET api/recipe/getallrecipes.
/// Defaults mirror the previous controller action.
/// </summary>
public class GetRecipesRequest : PagedSearchRequest
{
	[BindFrom("withingredient")]
	public bool WithIngredient { get; set; } = true;
}

/// <summary>Route parameter for endpoints keyed by a recipe id.</summary>
public class RecipeByIdRequest
{
	public Guid Id { get; set; }
}

/// <summary>Route parameter for endpoints keyed by a recipe-ingredient id.</summary>
public class RecipeItemByIdRequest
{
	public Guid Id { get; set; }
}