using ErrorOr;
using MediatR;
using FoodHub.DTOs;

namespace FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;
public class GetAllIngredientsQuery : IRequest<ErrorOr<List<IngredientDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
