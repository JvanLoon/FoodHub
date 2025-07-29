using MediatR;
using FoodHub.DTOs;
using ErrorOr;

namespace FoodCalc.Features.Recipes.Queries.GetById;
public record GetRecipeByIdQuery(Guid Id) : IRequest<ErrorOr<RecipeDto?>>;
