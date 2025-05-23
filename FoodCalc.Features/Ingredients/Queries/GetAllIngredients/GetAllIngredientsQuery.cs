using ErrorOr;

using MediatR;

namespace FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;
public class GetAllIngredientsQuery : IRequest<ErrorOr<List<FoodHub.Persistence.Entities.Ingredient>>>;
