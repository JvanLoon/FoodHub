using ErrorOr;

using MediatR;

namespace FoodCalc.Features.Recipes.Commands.DeleteRecipe;
public record DeleteRecipeCommand(Guid Id) : IRequest<ErrorOr<bool>>;
