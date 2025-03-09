using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recepts.Queries.GetAllRecepts;
public record GetAllReceptsQuery() : IRequest<ErrorOr<List<Recept>>>;
