using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recepts.Queries.GetAllRecepts;
public record GetAllRecipesQuery() : IRequest<ErrorOr<List<Recipe>>>;
