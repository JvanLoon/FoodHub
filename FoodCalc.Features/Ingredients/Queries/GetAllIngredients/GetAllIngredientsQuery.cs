using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Ingredients.Queries.GetAllIngredients;

public class GetAllIngredientsQuery : IRequest<ErrorOr<PagedResultDto<IngredientDto>>>, IPagedSearchQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public string? Search { get; set; }

    /// <summary>
    /// Caller's IdentityUser id. Unapproved catalog entries are returned only to whoever added
    /// them — see ReviewVisibilityExtensions.
    /// </summary>
    public string? RequestingUserId { get; set; }
}