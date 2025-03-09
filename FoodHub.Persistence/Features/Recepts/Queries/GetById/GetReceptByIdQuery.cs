using MediatR;
using FoodHub.Persistence.Entities;

namespace FoodCalc.ApiService.Features.Recepts.Queries.GetById;
public record GetReceptByIdQuery(Guid Id) : IRequest<Recept?>;
