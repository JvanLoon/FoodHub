using FastEndpoints;

using FluentValidation;

using FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;
using FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Users;

public class GetUsersRequest
{
	[BindFrom("page")]
	public int Page { get; set; } = 1;

	[BindFrom("pageSize")]
	public int PageSize { get; set; } = 25;

	[BindFrom("search")]
	public string? Search { get; set; }
}

/// <summary>Paging guard (PageSize lower-bounded only; see GetRecipesRequestValidator).</summary>
public class GetUsersRequestValidator : Validator<GetUsersRequest>
{
	public GetUsersRequestValidator()
	{
		RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
		RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
	}
}

/// <summary>GET api/user/users — Admin, Moderator or User.</summary>
public class GetUsersEndpoint(IMediator mediator)
	: Endpoint<GetUsersRequest, PagedResultDto<UserDto>>
{
	public override void Configure()
	{
		Get("api/user/users");
		Policies("Admin,Moderator,User");
	}

	public override async Task HandleAsync(GetUsersRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(new GetAllUsersQuery(req.Page, req.PageSize, req.Search), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(
				string.Join(", ", errors.Select(e => e.ToString())))));
	}
}

/// <summary>
/// GET api/user/allroles — Admin, Moderator or User.
/// Returns a raw comma-separated text/plain string (see the Admin variant).
/// </summary>
public class GetAllRolesEndpoint(IMediator mediator) : EndpointWithoutRequest
{
	public override void Configure()
	{
		Get("api/user/allroles");
		Policies("Admin,Moderator,User");
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
