using FastEndpoints;

using FoodCalc.Api.Common;
using FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;
using FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;

using MediatR;

namespace FoodCalc.Api.Endpoints.Users;

public class GetUsersRequest : IPagedSearchRequest
{
	[BindFrom("page")]
	public int Page { get; set; } = 1;

	[BindFrom("pageSize")]
	public int PageSize { get; set; } = 25;

	[BindFrom("search")]
	public string? Search { get; set; }
}

/// <summary>GET api/user/users — Admin, Moderator or User.</summary>
public class GetUsersEndpoint(IMediator mediator)
	: Endpoint<GetUsersRequest, PagedResultDto<UserDto>>
{
	public override void Configure()
	{
		Get(ApiRoutes.User.Users);
		Policies("Admin,Moderator,User");
	}

	public override async Task HandleAsync(GetUsersRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(new GetAllUsersQuery(req.Page, req.PageSize, req.Search), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => this.SendErrorsAsync(errors, ct: ct));
	}
}

/// <summary>
/// GET api/user/allroles — Admin, Moderator or User.
/// Returns a paged list of role names (see the Admin variant).
/// </summary>
public class GetAllRolesEndpoint(IMediator mediator)
	: Endpoint<GetRolesRequest, PagedResultDto<string>>
{
	public override void Configure()
	{
		Get(ApiRoutes.User.AllRoles);
		Policies("Admin,Moderator,User");
	}

	public override async Task HandleAsync(GetRolesRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(new GetAllRolesQuery(req.Page, req.PageSize, req.Search), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => this.SendErrorsAsync(errors, ct: ct));
	}
}
