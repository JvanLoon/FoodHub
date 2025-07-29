using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Recipes.Commands.AddRecipe;
public record AddRecipeCommand(CreateRecipeDto recipe) : IRequest<ErrorOr<RecipeDto>>;

