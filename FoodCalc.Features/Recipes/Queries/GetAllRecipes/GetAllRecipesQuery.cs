using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public record GetAllRecipesQuery(bool WithIngredient) : IRequest<ErrorOr<List<RecipeDto>>>;
