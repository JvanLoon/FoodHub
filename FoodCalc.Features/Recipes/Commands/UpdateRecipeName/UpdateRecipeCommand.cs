using ErrorOr;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipeName;

public record UpdateRecipeNameCommand(Guid RecipeId, string newRecipeName, ActingUser Acting)
    : IRequest<ErrorOr<RecipeDto>>;