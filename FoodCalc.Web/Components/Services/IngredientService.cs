using FoodHub.DTOs;

using Microsoft.AspNetCore.Components;

namespace FoodCalc.Web.Components.Services
{
	public class IngredientService
	{
		private readonly HttpClient _httpClient;

        public IngredientService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

		public async Task<List<IngredientDto>> GetAllIngredientsAsync()
		{
			var response = await _httpClient.GetAsync("api/ingredient");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<List<IngredientDto>>() ?? [];
		}

		public async Task<HttpResponseMessage> UpdateIngredient(UpdateIngredientDto ingredient)
		{
			var response = await _httpClient.PutAsJsonAsync("api/ingredient", ingredient);
			response.EnsureSuccessStatusCode();
			return response;
		}

		public async Task<bool> DeleteIngredient(Guid ingredientId)
		{
			var response = await _httpClient.DeleteAsync($"api/ingredient/deleteingredient/{ingredientId}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<bool>();
		}

		public async Task<HttpResponseMessage> AddIngredientAsync(CreateIngredientDto ingredient)
		{
			var response = await _httpClient.PostAsJsonAsync("api/ingredient", ingredient);
			response.EnsureSuccessStatusCode();
			return response;
		}
	}

}
