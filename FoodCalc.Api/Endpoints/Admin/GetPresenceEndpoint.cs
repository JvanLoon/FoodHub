using FastEndpoints;
using FoodCalc.Features.Authentication.Presence.Queries.GetPresence;
using MediatR;

namespace FoodCalc.Api.Endpoints.Admin;

/// <summary>
/// POST api/admin/presence — Admin or Moderator. Who of the given accounts is online.
///
/// Split out from GetUsers because the user list polls this every 30 seconds: re-running the
/// full query would mean a roles lookup per account on every tick, for two fields that are the
/// only thing that changes.
/// </summary>
public class GetPresenceEndpoint(IMediator mediator) : Endpoint<UserPresenceRequest, List<UserPresenceDto>>
{
    public override void Configure()
    {
        Post(ApiRoutes.Admin.Presence);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(UserPresenceRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPresenceQuery(req.UserIds), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}
