using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;

public record UpdateRecipeCommand(UpdateRecipeDto Recipe) : IRequest<ErrorOr<RecipeDto>>;