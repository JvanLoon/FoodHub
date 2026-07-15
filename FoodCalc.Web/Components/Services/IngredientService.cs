using FoodCalc.Web.Components.Services.Auth;
using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;
public class IngredientService(AuthenticatedHttpClientService httpClient)
{
	public Task<ApiResult<PagedResultDto<IngredientDto>>> GetPagedIngredientsAsync(int page, int pageSize, string? search = null)
	{
		var url = $"api/ingredient?page={page}&pageSize={pageSize}";
		if (!string.IsNullOrWhiteSpace(search))
			url += $"&search={Uri.EscapeDataString(search)}";

		return httpClient.GetAsync<PagedResultDto<IngredientDto>>(url);
	}

	public async Task<ApiResult<List<IngredientDto>>> GetAllIngredientsAsync()
	{
		var paged = await GetPagedIngredientsAsync(1, int.MaxValue);
		if (!paged.Success)
			return ApiResult<List<IngredientDto>>.Fail(paged.Error!, paged.StatusCode);

		return ApiResult<List<IngredientDto>>.Ok([.. paged.Data!.Items], paged.StatusCode);
	}

	public Task<ApiResult> UpdateIngredient(UpdateIngredientDto ingredient) =>
		httpClient.PutAsync("api/ingredient", ingredient);

	public Task<ApiResult> DeleteIngredient(Guid ingredientId) =>
		httpClient.DeleteAsync($"api/ingredient/deleteingredient/{ingredientId}");

	public Task<ApiResult> AddIngredientAsync(CreateIngredientDto ingredient) =>
		httpClient.PostAsync("api/ingredient", ingredient);
}
