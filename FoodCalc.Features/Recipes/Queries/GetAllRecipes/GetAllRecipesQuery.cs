using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;

/// <param name="RequestingUserId">
/// Caller's IdentityUser id. Unapproved recipes are returned only to their author, so this is
/// what keeps someone else's pending work out of the list — see ReviewVisibilityExtensions.
/// </param>
public record GetAllRecipesQuery(
    bool WithIngredient,
    string? RequestingUserId = null,
    int Page = 1,
    int PageSize = 25,
    string? Search = null) : IRequest<ErrorOr<PagedResultDto<RecipeDto>>>, IPagedSearchQuery;