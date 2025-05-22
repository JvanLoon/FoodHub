using ErrorOr;

using FoodHub.Persistence.Entities;

using MediatR;

namespace FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;
public record AddIngredientToRecipeCommand(RecipeIngredient RecipeIngredient) : IRequest<ErrorOr<RecipeIngredient>>;
