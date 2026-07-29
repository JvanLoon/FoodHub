using ErrorOr;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;

public record UpdateRecipeCommand(UpdateRecipeDto Recipe, ActingUser Acting) : IRequest<ErrorOr<RecipeDto>>;