using MediatR;
using FoodHub.Persistence.Entities;
using ErrorOr;

namespace FoodCalc.Features.Recipes.Queries.GetById;
public record GetRecipeByIdQuery(Guid Id) : IRequest<ErrorOr<Recipe?>>;
