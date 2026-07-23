using FastEndpoints;

namespace FoodCalc.Api.Endpoints.MealPlan;

/// <summary>Query parameters for GET api/mealplan — inclusive date range.</summary>
public class GetMealPlanRequest
{
	[BindFrom("from")]
	public DateOnly From { get; set; }

	[BindFrom("to")]
	public DateOnly To { get; set; }
}

/// <summary>Route parameter for endpoints keyed by a meal-plan entry id.</summary>
public class MealPlanEntryByIdRequest
{
	public Guid Id { get; set; }
}
