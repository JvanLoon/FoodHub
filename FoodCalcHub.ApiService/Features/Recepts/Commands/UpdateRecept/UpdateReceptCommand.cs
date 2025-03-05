using ErrorOr;

using FoodCalcHub.ApiService.Entities;
using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Commands.UpdateRecept;
public record UpdateReceptCommand(Recept Recept) : IRequest<ErrorOr<Recept>>;

