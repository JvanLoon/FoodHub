using FoodHub.DTOs;

using Microsoft.AspNetCore.Components;

namespace FoodCalc.Web.Components.Services
{
	public class IngredientService(HttpClient httpClient, NavigationManager navigationManager)
	{
		public NavigationManager navigationManager { get; set; } = navigationManager;

		public async Task<List<IngredientDto>> GetAllIngredientsAsync()
		{
			var response = await httpClient.GetAsync("api/ingredient");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<List<IngredientDto>>() ?? new List<IngredientDto>();
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

}
