using FastEndpoints;

using FoodCalc.Api.Common;
using FoodCalc.Features.MealPlan.Commands.DeleteMealPlanEntry;

using MediatR;

namespace FoodCalc.Api.Endpoints.MealPlan;

/// <summary>DELETE api/mealplan/{id} — remove one of the calling user's own entries.</summary>
public class DeleteMealPlanEntryEndpoint(IMediator mediator) : Endpoint<MealPlanEntryByIdRequest, bool>
{
	public override void Configure()
	{
		Delete(ApiRoutes.MealPlan.DeleteTemplate);
		Policies("Admin,Moderator,User");
	}

	public override async Task HandleAsync(MealPlanEntryByIdRequest req, CancellationToken ct)
	{
		var userId = User.GetUserId();
		if (string.IsNullOrEmpty(userId))
		{
			await Send.StringAsync("No user in token", 401, cancellation: ct);
			return;
		}

		var result = await mediator.Send(new DeleteMealPlanEntryCommand(userId, req.Id), ct);

		await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
	}
}
