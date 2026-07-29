using ErrorOr;
using MediatR;

namespace FoodCalc.Features.Review.Commands.RejectContent;

/// <summary>
/// Rejects a pending recipe, catalog ingredient, or single recipe line by deleting it. No
/// reason is captured and nothing is recorded — rejection is simply removal now.
/// </summary>
public record RejectContentCommand(ReviewTargetType TargetType, Guid TargetId) : IRequest<ErrorOr<bool>>;
