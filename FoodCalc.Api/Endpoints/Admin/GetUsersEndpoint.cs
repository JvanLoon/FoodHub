using FastEndpoints;
using FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;
using MediatR;

namespace FoodCalc.Api.Endpoints.Admin;

/// <summary>GET api/admin/users — Admin or Moderator.</summary>
public class GetUsersEndpoint(IMediator mediator) : Endpoint<GetUsersRequest, PagedResultDto<UserDto>>
{
    public override void Configure()
    {
        Get(ApiRoutes.Admin.Users);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(GetUsersRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllUsersQuery(req.Page, req.PageSize, req.Search), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}