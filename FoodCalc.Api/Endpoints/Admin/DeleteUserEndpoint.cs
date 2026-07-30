using FastEndpoints;
using FoodCalc.Features.Authentication.Users.Commands.DeleteUser;
using MediatR;

namespace FoodCalc.Api.Endpoints.Admin;

/// <summary>Query-bound because DELETE carries no body (see <see cref="RemoveUserRoleRequest"/>).</summary>
public class DeleteUserRequest
{
    [BindFrom("email")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DELETE api/admin/user?email= — Admin. Removes the account and its meal calendar; the
/// handler refuses to delete the caller's own account.
/// </summary>
public class DeleteUserEndpoint(IMediator mediator) : Endpoint<DeleteUserRequest>
{
    public override void Configure()
    {
        Delete(ApiRoutes.Admin.User);
        Policies("Admin");
    }

    public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            await Send.StringAsync(ResponseMessages.Token.NoUserInToken, 401, cancellation: ct);
            return;
        }

        var result = await mediator.Send(new DeleteUserCommand(req.Email, userId), ct);

        await result.Match(_ => Send.OkAsync(cancellation: ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}
