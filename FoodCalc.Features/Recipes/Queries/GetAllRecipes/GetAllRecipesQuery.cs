using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public record GetAllRecipesQuery(bool WithIngredient, int Page = 1, int PageSize = 25, string? Search = null) : IRequest<ErrorOr<PagedResultDto<RecipeDto>>>;
