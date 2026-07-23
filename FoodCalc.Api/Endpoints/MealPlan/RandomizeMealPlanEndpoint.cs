using FastEndpoints;

using FoodCalc.Api.Common;
using FoodCalc.Features.MealPlan.Commands.RandomizeMealPlan;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.MealPlan;

/// <summary>POST api/mealplan/randomize — fill the selected days with random recipes.</summary>
public class RandomizeMealPlanEndpoint(IMediator mediator) : Endpoint<RandomizeMealPlanDto, List<MealPlanEntryDto>>
{
	public override void Configure()
	{
		Post(ApiRoutes.MealPlan.Randomize);
		Policies("Admin,Moderator,User");
	}

	public override async Task HandleAsync(RandomizeMealPlanDto req, CancellationToken ct)
	{
		var userId = User.GetUserId();
		if (string.IsNullOrEmpty(userId))
		{
			await Send.StringAsync("No user in token", 401, cancellation: ct);
			return;
		}

		var result = await mediator.Send(
			new RandomizeMealPlanCommand(userId, req.Dates, req.Ingredients, req.RecipesPerDay, req.Overwrite), ct);

		await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
	}
}
