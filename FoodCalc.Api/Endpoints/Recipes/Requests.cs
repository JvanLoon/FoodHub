using FastEndpoints;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>
/// Query parameters for GET api/recipe/getallrecipes.
/// Defaults mirror the previous controller action.
/// </summary>
public class GetRecipesRequest : IPagedSearchRequest
{
	[BindFrom("withingredient")]
	public bool WithIngredient { get; set; } = true;

	[BindFrom("page")]
	public int Page { get; set; } = 1;

	[BindFrom("pageSize")]
	public int PageSize { get; set; } = 25;

	[BindFrom("search")]
	public string? Search { get; set; }
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