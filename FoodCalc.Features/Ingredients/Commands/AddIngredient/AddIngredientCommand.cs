using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Ingredients.Commands.AddIngredient;
public record AddIngredientCommand(Ingredient Ingredient) : IRequest<ErrorOr<Ingredient>>;
