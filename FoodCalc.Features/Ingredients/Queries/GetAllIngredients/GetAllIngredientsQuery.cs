using ErrorOr;
using MediatR;
using FoodHub.DTOs;

namespace FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;
public class GetAllIngredientsQuery : IRequest<ErrorOr<List<IngredientDto>>>;
