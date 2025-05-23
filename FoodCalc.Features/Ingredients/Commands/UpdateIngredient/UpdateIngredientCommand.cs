using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Ingredients.Commands.UpdateIngredient;
public record UpdateIngredientCommand(Ingredient Ingredient) : IRequest<ErrorOr<Ingredient>>;

