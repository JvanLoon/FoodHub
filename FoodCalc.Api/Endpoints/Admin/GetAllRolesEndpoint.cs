using FastEndpoints;

using FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;

using MediatR;

namespace FoodCalc.Api.Endpoints.Admin;

/// <summary>
/// GET api/admin/allroles — Admin or Moderator.
/// Returns the roles as a raw comma-separated text/plain string, matching the
/// old controller (the Blazor AdminService reads the body as a string and
/// splits on ',', so this must NOT be a JSON-encoded string).
/// </summary>
public class GetAllRolesEndpoint(IMediator mediator) : EndpointWithoutRequest
{
	public override void Configure()
	{
		Get(ApiRoutes.Admin.AllRoles);
		Policies("Admin,Moderator");
	}

	public override async Task HandleAsync(CancellationToken ct)
	{
		var result = await mediator.Send(new GetAllRolesQuery(), ct);

		await result.Match(
			roles => Send.StringAsync(string.Join(",", roles), cancellation: ct),
			errors => Send.ResultAsync(TypedResults.Problem(
				string.Join(", ", errors.Select(e => e.ToString())))));
	}
}
