using FoodCalc.Web.Components.Services.Auth;
using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;
public class IngredientService(AuthenticatedHttpClientService httpClient)
{
	public async Task<PagedResultDto<IngredientDto>> GetPagedIngredientsAsync(int page, int pageSize, string? search = null)
	{
		var url = $"api/ingredient?page={page}&pageSize={pageSize}";
		if (!string.IsNullOrWhiteSpace(search))
			url += $"&search={Uri.EscapeDataString(search)}";

		var response = await httpClient.GetAsync(url);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<PagedResultDto<IngredientDto>>() ?? new();
	}

	public async Task<List<IngredientDto>> GetAllIngredientsAsync()
	{
		var paged = await GetPagedIngredientsAsync(1, int.MaxValue);
		return [..paged.Items];
	}

	public async Task<HttpResponseMessage> UpdateIngredient(UpdateIngredientDto ingredient)
	{
		var response = await httpClient.PutAsJsonAsync("api/ingredient", ingredient);
		response.EnsureSuccessStatusCode();
		return response;
	}

	public async Task<bool> DeleteIngredient(Guid ingredientId)
	{
		var response = await httpClient.DeleteAsync($"api/ingredient/deleteingredient/{ingredientId}");
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<bool>();
	}

	public async Task<HttpResponseMessage> AddIngredientAsync(CreateIngredientDto ingredient)
	{
		var response = await httpClient.PostAsJsonAsync("api/ingredient", ingredient);
		response.EnsureSuccessStatusCode();
		return response;
	}
}
