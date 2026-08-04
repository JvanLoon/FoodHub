using FastEndpoints;

namespace FoodCalc.Api.Endpoints.Authentication;

/// <summary>
/// POST api/authentication/heartbeat — any signed-in role. Deliberately empty: PresenceMiddleware
/// stamps every authenticated request, so simply arriving here is the whole payload. It exists so
/// a user reading a page without clicking anything still counts as online.
/// </summary>
public class HeartbeatEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post(ApiRoutes.Authentication.Heartbeat);
        Policies("Admin,Moderator,User");
    }

    public override async Task HandleAsync(CancellationToken ct) => await Send.NoContentAsync(ct);
}
