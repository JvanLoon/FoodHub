using FastEndpoints;

using FoodCalc.Api.Common;
using FoodCalc.Features.MealPlan.Commands.AddMealPlanEntry;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.MealPlan;

/// <summary>POST api/mealplan — add a recipe to a day on the calling user's calendar.</summary>
public class AddMealPlanEntryEndpoint(IMediator mediator) : Endpoint<AddMealPlanEntryDto, MealPlanEntryDto>
{
	public override void Configure()
	{
		Post(ApiRoutes.MealPlan.Add);
		Policies("Admin,Moderator,User");
	}

	public override async Task HandleAsync(AddMealPlanEntryDto req, CancellationToken ct)
	{
		var userId = User.GetUserId();
		if (string.IsNullOrEmpty(userId))
		{
			await Send.StringAsync("No user in token", 401, cancellation: ct);
			return;
		}

		var result = await mediator.Send(new AddMealPlanEntryCommand(userId, req.Date, req.RecipeId), ct);

		await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
	}
}
