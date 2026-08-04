using ErrorOr;
using MediatR;

namespace FoodCalc.Features.Authentication.Presence.Commands.TouchPresence;

/// <summary>
/// Records that <see cref="UserId"/> was active just now. <see cref="IsOnline"/> is false only
/// when the user signed out explicitly, which takes the dot away immediately instead of letting
/// it fade out over the presence window.
/// </summary>
public record TouchPresenceCommand(string UserId, bool IsOnline = true) : IRequest<ErrorOr<bool>>;
