using FastEndpoints;

namespace FoodCalc.Api.Common;

/// <summary>
/// The page/pageSize/search triplet every list endpoint takes, declared once.
///
/// It stays in the API rather than moving to FoodHub.DTOs because these arrive as query
/// string values on GET requests — a GET has no body to share a type through, and the
/// <c>[BindFrom]</c> attributes that bind them are FastEndpoints', which that project
/// deliberately does not reference.
///
/// Derive and add properties for endpoints that need more (see
/// <see cref="FoodCalc.Api.Endpoints.Recipes.GetRecipesRequest"/>).
/// </summary>
public abstract class PagedSearchRequest : IPagedSearchRequest
{
    [BindFrom("page")]
    public int Page { get; set; } = 1;

    [BindFrom("pageSize")]
    public int PageSize { get; set; } = 25;

    [BindFrom("search")]
    public string? Search { get; set; }
}