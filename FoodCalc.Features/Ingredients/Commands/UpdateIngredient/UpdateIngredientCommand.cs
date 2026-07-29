using ErrorOr;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Ingredients.Commands.UpdateIngredient;

public record UpdateIngredientCommand(UpdateIngredientDto Ingredient, ActingUser Acting)
    : IRequest<ErrorOr<IngredientDto>>;