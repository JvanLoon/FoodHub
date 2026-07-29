using ErrorOr;
using FoodHub.Persistence.Entities;
using MediatR;

namespace FoodCalc.Features.Review.Commands.RejectContent;

/// <param name="Reason">Required explanation, stored for the author.</param>
/// <param name="Delete">True to remove the rejected item; false to leave it with its author.</param>
/// <param name="RejectedByUserId">The moderator, recorded on the rejection.</param>
public record RejectContentCommand(
    ReviewTargetType TargetType,
    Guid TargetId,
    string Reason,
    bool Delete,
    string? RejectedByUserId) : IRequest<ErrorOr<bool>>;
