using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Recipes.Commands.AddRecipe;

public record AddRecipeCommand(CreateRecipeDto recipe, string? CreatedByUserId = null) : IRequest<ErrorOr<RecipeDto>>;