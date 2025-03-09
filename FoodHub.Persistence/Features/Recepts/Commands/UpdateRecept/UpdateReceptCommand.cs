using ErrorOr;

using FoodHub.Persistence.Entities;
using MediatR;

namespace FoodCalc.ApiService.Features.Recepts.Commands.UpdateRecept;
public record UpdateReceptCommand(Recept Recept) : IRequest<ErrorOr<Recept>>;

