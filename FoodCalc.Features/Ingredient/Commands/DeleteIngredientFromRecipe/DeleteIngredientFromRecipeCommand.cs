using ErrorOr;

using MediatR;

namespace FoodCalc.Features.Recipes.Commands.DeleteIngredientFromRecipe;
public record DeleteIngredientFromRecipeCommand(Guid Id) : IRequest<ErrorOr<bool>>;
