using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public record GetAllRecipesQuery() : IRequest<ErrorOr<List<Recipe>>>;
