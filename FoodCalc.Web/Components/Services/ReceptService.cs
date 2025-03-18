using System.Net.Http.Json;

using FoodHub.Persistence.Entities;
using Microsoft.AspNetCore.Components;

namespace FoodCalc.Client.Services
{
	public class ReceptService(HttpClient httpClient, NavigationManager navigationManager)
	{
		public NavigationManager navigationManager { get; set; } = navigationManager;

		public async Task<List<Recept>> GetAllReceptsAsync()
		{
			var response = await httpClient.GetAsync("api/recept");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<List<Recept>>();
		}

		public async Task<HttpResponseMessage> AddRecept(Recept recept)
		{
			var response = await httpClient.PostAsJsonAsync("api/recept", recept);
			response.EnsureSuccessStatusCode();
			return response;
		}

		public async Task<HttpResponseMessage> UpdateRecept(Recept recept)
		{
			var response = await httpClient.PutAsJsonAsync("api/recept", recept);
			response.EnsureSuccessStatusCode();
			return response;
		}

		public async Task<List<Ingredient>> GetIngredientsAsync()
		{
			var response = await httpClient.GetAsync("api/recept/ingredients");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<List<Ingredient>>();
		}

		public async Task<HttpResponseMessage> AddIngredient(Ingredient ingredient)
		{
			var response = await httpClient.PostAsJsonAsync("api/recept/ingredient", ingredient);
			response.EnsureSuccessStatusCode();
			return response;
		}
	}
}
