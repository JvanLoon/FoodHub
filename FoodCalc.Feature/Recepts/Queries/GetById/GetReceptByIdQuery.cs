using MediatR;
using FoodHub.Persistence.Entities;
using ErrorOr;

namespace FoodCalc.Features.Recepts.Queries.GetById;
public record GetReceptByIdQuery(Guid Id) : IRequest<ErrorOr<Recept?>>;
