using FoodCalcHub.ApiService.Entities;
using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Queries.GetAllRecepts;
public record GetAllReceptsQuery() : IRequest<IEnumerable<Recept>>;