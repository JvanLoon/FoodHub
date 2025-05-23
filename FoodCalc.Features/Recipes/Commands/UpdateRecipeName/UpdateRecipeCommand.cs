using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;
public record UpdateRecipeNameCommand(Guid RecipeId, string newRecipeName) : IRequest<ErrorOr<Recipe>>;

