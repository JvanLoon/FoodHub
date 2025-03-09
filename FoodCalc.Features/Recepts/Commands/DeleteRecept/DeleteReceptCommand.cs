using ErrorOr;
using MediatR;

namespace FoodCalc.Features.Recepts.Commands.DeleteRecept;
public record DeleteReceptCommand(Guid Id) : IRequest<ErrorOr<bool>>;
