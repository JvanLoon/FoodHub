
using ErrorOr;

using MediatR;

namespace FoodCalc.Feature.Ingredient.Commands.AddIngredient;

public record AddIngredientCommand(FoodHub.Persistence.Entities.Ingredient Ingredient)
	: IRequest<ErrorOr<Success>>;
