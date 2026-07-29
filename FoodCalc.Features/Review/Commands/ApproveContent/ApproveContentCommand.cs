using ErrorOr;
using MediatR;

namespace FoodCalc.Features.Review.Commands.ApproveContent;

/// <summary>
/// Publishes a pending recipe or catalog ingredient, or clears the changed flag on one recipe
/// line. Callable only by Admin and Moderator — enforced at the endpoint, since approval is a
/// role capability rather than a question of content ownership.
/// </summary>
public record ApproveContentCommand(ReviewTargetType TargetType, Guid TargetId) : IRequest<ErrorOr<bool>>;
