using ErrorOr;
using FoodCalc.Features.Review;
using MediatR;

namespace FoodCalc.Features.Recipes.Commands.DeleteRecipe;

public record DeleteRecipeCommand(Guid Id, ActingUser Acting) : IRequest<ErrorOr<bool>>;