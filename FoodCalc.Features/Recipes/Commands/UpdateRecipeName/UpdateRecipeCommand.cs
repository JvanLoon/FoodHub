using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipeName;

public record UpdateRecipeNameCommand(Guid RecipeId, string newRecipeName) : IRequest<ErrorOr<RecipeDto>>;
