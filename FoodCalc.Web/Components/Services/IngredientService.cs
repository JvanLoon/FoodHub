using FoodHub.Persistence.Entities;

using Microsoft.AspNetCore.Components;

namespace FoodCalc.Web.Components.Services
{
	public class IngredientService(HttpClient httpClient, NavigationManager navigationManager)
	{
		public NavigationManager navigationManager { get; set; } = navigationManager;

		public async Task<List<Ingredient>> GetAllIngredientsAsync()
		{
			var response = await httpClient.GetAsync("api/ingredient");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<List<Ingredient>>();
		}

		public async Task<HttpResponseMessage> AddIngredientAsync(Ingredient ingredient)
		{
			var response = await httpClient.PostAsJsonAsync("api/ingredient", ingredient);
			response.EnsureSuccessStatusCode();
			return response;
		}
	}

}
