using FastEndpoints;
using FoodCalc.Api.Common;
using FoodCalc.Api.Middleware;
using FoodCalc.Features.Authentication.Presence.Commands.TouchPresence;
using MediatR;

namespace FoodCalc.Api.Endpoints.Authentication;

/// <summary>
/// POST api/authentication/signout — any signed-in role. Flips the caller offline immediately.
///
/// The JWT itself is stateless and is simply dropped by the client, so there is no server session
/// to end; this exists only so an admin sees the dot go out the moment someone clicks Uitloggen
/// rather than up to three minutes later.
/// </summary>
public class SignOutPresenceEndpoint(IMediator mediator, PresenceThrottle throttle) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post(ApiRoutes.Authentication.SignOut);
        Policies("Admin,Moderator,User");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // PresenceMiddleware stamps every authenticated request on the way out, including this
        // one — it would put the user straight back online. Hand it the decision instead.
        HttpContext.Items[PresenceMiddleware.SkipKey] = true;

        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            await Send.NoContentAsync(ct);
            return;
        }

        // Cleared so the next sign-in writes straight away instead of waiting out a throttle slot
        // claimed moments ago by this same account.
        throttle.Clear(userId);
        await mediator.Send(new TouchPresenceCommand(userId, IsOnline: false), ct);

        await Send.NoContentAsync(ct);
    }
}
