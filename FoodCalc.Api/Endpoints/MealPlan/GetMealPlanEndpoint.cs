using FastEndpoints;

using FoodCalc.Api.Common;
using FoodCalc.Features.MealPlan.Queries.GetMealPlan;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.MealPlan;

/// <summary>GET api/mealplan?from=&amp;to= — the calling user's calendar entries in the range.</summary>
public class GetMealPlanEndpoint(IMediator mediator)
	: Endpoint<GetMealPlanRequest, List<MealPlanEntryDto>>
{
	public override void Configure()
	{
		Get(ApiRoutes.MealPlan.GetRange);
		Policies("Admin,Moderator,User");
	}

	public override async Task HandleAsync(GetMealPlanRequest req, CancellationToken ct)
	{
		var userId = User.GetUserId();
		if (string.IsNullOrEmpty(userId))
		{
			await Send.StringAsync("No user in token", 401, cancellation: ct);
			return;
		}

		var result = await mediator.Send(new GetMealPlanQuery(userId, req.From, req.To), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => this.SendErrorsAsync(errors, ct: ct));
	}
}
