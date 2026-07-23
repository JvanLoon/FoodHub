using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;

public record AddIngredientToRecipeCommand(RecipeItemDto RecipeItem) : IRequest<ErrorOr<RecipeItemDto>>;