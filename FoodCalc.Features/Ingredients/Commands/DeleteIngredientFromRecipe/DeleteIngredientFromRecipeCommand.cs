using ErrorOr;
using FoodCalc.Features.Review;
using MediatR;

namespace FoodCalc.Features.Ingredients.Commands.DeleteIngredientFromRecipe;

public record DeleteIngredientFromRecipeCommand(Guid Id, ActingUser Acting) : IRequest<ErrorOr<bool>>;