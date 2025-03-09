using ErrorOr;
using FoodHub.Persistence.Entities;
using MediatR;

namespace FoodCalc.ApiService.Features.Recepts.Commands.AddRecept;
public record AddReceptCommand(Recept recept) : IRequest<ErrorOr<Recept>>;

