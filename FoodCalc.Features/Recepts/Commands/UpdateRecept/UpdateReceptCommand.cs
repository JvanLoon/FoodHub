using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recepts.Commands.UpdateRecept;
public record UpdateReceptCommand(Recept Recept) : IRequest<ErrorOr<Recept>>;

