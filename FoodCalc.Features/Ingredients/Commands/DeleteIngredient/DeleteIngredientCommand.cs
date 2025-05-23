using ErrorOr;

using MediatR;

namespace FoodCalc.Features.Ingredients.Commands.DeleteIngredient;
public record DeleteIngredientCommand(Guid Id) : IRequest<ErrorOr<bool>>;
