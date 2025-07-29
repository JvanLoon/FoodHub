using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;
public record UpdateRecipeCommand(UpdateRecipeDto Recipe) : IRequest<ErrorOr<RecipeDto>>;

