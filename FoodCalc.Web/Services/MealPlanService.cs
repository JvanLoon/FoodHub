using FoodCalc.Web.Services.Auth;

using FoodHub.DTOs;

namespace FoodCalc.Web.Services;

/// <summary>Client for the per-user meal calendar (api/mealplan).</summary>
public class MealPlanService(AuthenticatedHttpClientService httpClient)
{
	private const string DateFormat = "yyyy-MM-dd";

	public Task<ApiResult<List<MealPlanEntryDto>>> GetRangeAsync(DateOnly from, DateOnly to)
	{
		var url = $"{ApiRoutes.MealPlan.GetRange}?from={from.ToString(DateFormat)}&to={to.ToString(DateFormat)}";
		return httpClient.GetAsync<List<MealPlanEntryDto>>(url);
	}

	public Task<ApiResult<MealPlanEntryDto>> AddAsync(DateOnly date, Guid recipeId)
	{
		var payload = new AddMealPlanEntryDto {Date = date, RecipeId = recipeId};
		return httpClient.PostAsync<AddMealPlanEntryDto, MealPlanEntryDto>(ApiRoutes.MealPlan.Add, payload);
	}

	public Task<ApiResult> DeleteAsync(Guid id) => httpClient.DeleteAsync(ApiRoutes.MealPlan.Delete(id));

	public Task<ApiResult<List<MealPlanEntryDto>>> RandomizeAsync(RandomizeMealPlanDto request) =>
		httpClient.PostAsync<RandomizeMealPlanDto, List<MealPlanEntryDto>>(ApiRoutes.MealPlan.Randomize, request);
}
