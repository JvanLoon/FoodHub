using ErrorOr;

using MediatR;

namespace FoodCalc.Feature.Ingredient.Queries.GetAllIngredients;
public class GetAllIngredientsQuery : IRequest<ErrorOr<List<FoodHub.Persistence.Entities.Ingredient>>>;
