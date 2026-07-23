using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Ingredients.Commands.AddIngredient;

public record AddIngredientCommand(CreateIngredientDto Ingredient) : IRequest<ErrorOr<IngredientDto>>;