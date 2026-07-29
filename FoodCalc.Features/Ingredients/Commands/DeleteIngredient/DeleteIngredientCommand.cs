using ErrorOr;
using FoodCalc.Features.Review;
using MediatR;

namespace FoodCalc.Features.Ingredients.Commands.DeleteIngredient;

public record DeleteIngredientCommand(Guid Id, ActingUser Acting) : IRequest<ErrorOr<bool>>;