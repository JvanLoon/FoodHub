using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Ingredients.Commands.UpdateIngredient;
public record UpdateIngredientCommand(UpdateIngredientDto Ingredient) : IRequest<ErrorOr<IngredientDto>>;

