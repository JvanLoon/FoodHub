using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Ingredients.Commands.AddIngredient;
public record AddIngredientCommand(CreateIngredientDto Ingredient) : IRequest<ErrorOr<IngredientDto>>;
