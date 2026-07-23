using FoodCalc.Web.Services.Auth;

using FoodHub.DTOs;

namespace FoodCalc.Web.Services;

public class IngredientService(AuthenticatedHttpClientService httpClient)
{
	public Task<ApiResult<PagedResultDto<IngredientDto>>> GetPagedIngredientsAsync(
		int page,
		int pageSize,
		string? search = null
	)
	{
		var url = $"{ApiRoutes.Ingredient.GetAll}?page={page}&pageSize={pageSize}";
		if (!string.IsNullOrWhiteSpace(search))
			url += $"&search={Uri.EscapeDataString(search)}";

		return httpClient.GetAsync<PagedResultDto<IngredientDto>>(url);
	}

	public async Task<ApiResult<List<IngredientDto>>> GetAllIngredientsAsync()
	{
		var paged = await GetPagedIngredientsAsync(1, int.MaxValue);
		if (!paged.Success)
			return ApiResult<List<IngredientDto>>.Fail(paged.Errors, paged.StatusCode);

		return ApiResult<List<IngredientDto>>.Ok([..paged.Data!.Items], paged.StatusCode);
	}

	public Task<ApiResult> UpdateIngredient(UpdateIngredientDto ingredient) =>
		httpClient.PutAsync(ApiRoutes.Ingredient.Update, ingredient);

	public Task<ApiResult> DeleteIngredient(Guid ingredientId) =>
		httpClient.DeleteAsync(ApiRoutes.Ingredient.Delete(ingredientId));

	public Task<ApiResult> AddIngredientAsync(CreateIngredientDto ingredient) =>
		httpClient.PostAsync(ApiRoutes.Ingredient.Create, ingredient);
}
