using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.Recipes.Commands.AddRecipe;

public record AddRecipeCommand(CreateRecipeDto recipe) : IRequest<ErrorOr<RecipeDto>>;

