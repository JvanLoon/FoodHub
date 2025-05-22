using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;
public record UpdateRecipeCommand(Recipe Recipe) : IRequest<ErrorOr<Recipe>>;

