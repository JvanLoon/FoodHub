using ErrorOr;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;

public record AddIngredientToRecipeCommand(RecipeItemDto RecipeItem, ActingUser Acting)
    : IRequest<ErrorOr<RecipeItemDto>>;