using MediatR;
using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recepts.Queries.GetById;
public record GetReceptByIdQuery(Guid Id) : IRequest<Recept?>;
