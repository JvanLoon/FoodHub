using ErrorOr;
using FoodHub.Persistence.Entities;
using MediatR;

namespace FoodCalc.Features.Recepts.Queries.GetAllRecepts;
public record GetAllReceptsQuery() : IRequest<ErrorOr<List<Recept>>>;
