using ErrorOr;

using MediatR;

namespace FoodCalc.ApiService.Features.Recepts.Commands.DeleteRecept;
public record DeleteReceptCommand(Guid Id): IRequest<ErrorOr<bool>>;
