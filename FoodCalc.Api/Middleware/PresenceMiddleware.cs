using FoodCalc.Api.Common;
using FoodCalc.Features.Authentication.Presence.Commands.TouchPresence;
using MediatR;

namespace FoodCalc.Api.Middleware;

/// <summary>
/// Stamps "last seen" for the authenticated caller. Sitting in front of the endpoints rather than
/// inside them means every authenticated call counts as activity, so a user who is genuinely
/// using the app stays online on their own traffic and the client heartbeat only has to cover
/// idle time.
///
/// Runs after UseAuthentication, and after the response so a slow presence write never delays
/// the caller.
/// </summary>
public class PresenceMiddleware(RequestDelegate next, PresenceThrottle throttle, ILogger<PresenceMiddleware> logger)
{
    /// <summary>
    /// Set on <see cref="HttpContext.Items"/> by an endpoint that has already decided the user's
    /// presence itself. Without it the sign-out endpoint would mark the user offline and then be
    /// overwritten by this middleware on the way back out of the very same request.
    /// </summary>
    public const string SkipKey = "Presence.Skip";

    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (context.Items.ContainsKey(SkipKey))
            return;

        var userId = context.User.GetUserId();
        if (string.IsNullOrEmpty(userId) || context.User.Identity?.IsAuthenticated != true)
            return;

        if (!throttle.ShouldWrite(userId))
            return;

        try
        {
            // Resolved per request rather than injected: this middleware is a singleton and the
            // mediator's handlers need the scoped DbContext.
            var mediator = context.RequestServices.GetRequiredService<IMediator>();

            // Not passing the request's CancellationToken on purpose — the response has already
            // gone out, and a client that disconnects immediately after would otherwise cancel
            // the very write that proves it was there.
            await mediator.Send(new TouchPresenceCommand(userId));
        }
        catch (Exception ex)
        {
            // The response is already sent; there is nothing left to fail into.
            logger.LogWarning(ex, "Presence update failed for user {UserId}", userId);
        }
    }
}
