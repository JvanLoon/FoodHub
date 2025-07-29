using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipeName;
public record UpdateRecipeNameCommand(Guid RecipeId, string newRecipeName) : IRequest<ErrorOr<RecipeDto>>;

