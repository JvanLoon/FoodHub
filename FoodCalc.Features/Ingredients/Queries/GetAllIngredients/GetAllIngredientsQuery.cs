using ErrorOr;
using FoodCalc.Features;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;

public class GetAllIngredientsQuery : IRequest<ErrorOr<PagedResultDto<IngredientDto>>>, IPagedSearchQuery
{
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 25;
	public string? Search { get; set; }
}