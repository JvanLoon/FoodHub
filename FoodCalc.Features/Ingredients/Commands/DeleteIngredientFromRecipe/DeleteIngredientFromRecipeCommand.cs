using ErrorOr;

using MediatR;

namespace FoodCalc.Features.Ingredients.Commands.DeleteIngredientFromRecipe;
public record DeleteIngredientFromRecipeCommand(Guid Id) : IRequest<ErrorOr<bool>>;
