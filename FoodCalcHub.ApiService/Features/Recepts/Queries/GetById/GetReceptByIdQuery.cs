using MediatR;
using FoodCalcHub.ApiService.Entities;

namespace FoodCalcHub.ApiService.Features.Recepts.Queries.GetById;
public record GetReceptByIdQuery(Guid Id) : IRequest<Recept?>;