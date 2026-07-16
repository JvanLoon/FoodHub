using FastEndpoints;

using FoodCalc.Api.Endpoints.Common;
using FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Admin;

/// <summary>
/// GET api/admin/allroles — Admin or Moderator.
/// Returns a paged list of role names. The Blazor AdminService fetches them all
/// via pageSize = int.MaxValue and reads Items.
/// </summary>
public class GetAllRolesEndpoint(IMediator mediator)
	: Endpoint<GetRolesRequest, PagedResultDto<string>>
{
	public override void Configure()
	{
		Get(ApiRoutes.Admin.AllRoles);
		Policies("Admin,Moderator");
	}

	public override async Task HandleAsync(GetRolesRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(new GetAllRolesQuery(req.Page, req.PageSize, req.Search), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(
				string.Join(", ", errors.Select(e => e.ToString())))));
	}
}
