using ErrorOr;

using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Commands.DeleteRecept;
public record DeleteReceptCommand(Guid Id): IRequest<ErrorOr<bool>>;