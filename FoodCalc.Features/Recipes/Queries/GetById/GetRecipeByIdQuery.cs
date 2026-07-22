using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.Recipes.Queries.GetById;

public record GetRecipeByIdQuery(Guid Id) : IRequest<ErrorOr<RecipeDto?>>;
