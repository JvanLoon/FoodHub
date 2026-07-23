using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.Ingredients.Commands.UpdateIngredient;

public record UpdateIngredientCommand(UpdateIngredientDto Ingredient) : IRequest<ErrorOr<IngredientDto>>;
