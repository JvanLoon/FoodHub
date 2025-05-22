using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recipes.Commands.AddRecipe;
public record AddRecipeCommand(Recipe recipe) : IRequest<ErrorOr<Recipe>>;

