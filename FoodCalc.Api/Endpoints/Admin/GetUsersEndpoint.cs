using FastEndpoints;

using FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Admin;

/// <summary>Query parameters for the paged user list endpoints.</summary>
public class GetUsersRequest
{
	[BindFrom("page")]
	public int Page { get; set; } = 1;

	[BindFrom("pageSize")]
	public int PageSize { get; set; } = 25;

	[BindFrom("search")]
	public string? Search { get; set; }
}

/// <summary>GET api/admin/users — Admin or Moderator.</summary>
public class GetUsersEndpoint(IMediator mediator)
	: Endpoint<GetUsersRequest, PagedResultDto<UserDto>>
{
	public override void Configure()
	{
		Get("api/admin/users");
		Policies("Admin,Moderator");
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
