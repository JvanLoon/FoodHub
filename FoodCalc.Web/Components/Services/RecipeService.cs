using System.Net.Http.Json;

using FoodHub.Persistence.Entities;
using Microsoft.AspNetCore.Components;

namespace FoodCalc.Web.Components.Services
{
	public class RecipeService(HttpClient httpClient, NavigationManager navigationManager)
	{
		public NavigationManager navigationManager { get; set; } = navigationManager;

		public async Task<List<Recipe>> GetAllRecipesAsync()
		{
			var response = await httpClient.GetAsync("api/recept");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<List<Recipe>>();
		}

		public async Task<Recipe> GetRecipeByIdAsync(Guid recipeId)
		{
			var response = await httpClient.GetAsync($"api/recept/{recipeId}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<Recipe>();
		}

		public async Task<HttpResponseMessage> AddRecipe(Recipe recipe)
		{
			var response = await httpClient.PostAsJsonAsync("api/recept", recipe);
			response.EnsureSuccessStatusCode();
			return response;
		}

		public async Task<HttpResponseMessage> UpdateRecipe(Recipe recipe)
		{
			var response = await httpClient.PutAsJsonAsync("api/recept", recipe);
			response.EnsureSuccessStatusCode();
			return response;
		}

		public async Task<List<Ingredient>> GetIngredientsAsync()
		{
			var response = await httpClient.GetAsync("api/recept/ingredients");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<List<Ingredient>>();
		}

		public async Task<HttpResponseMessage> AddIngredient(RecipeIngredient ingredient)
		{
			var response = await httpClient.PostAsJsonAsync("api/recept/ingredient", ingredient);
			response.EnsureSuccessStatusCode();
			return response;
		}
	}
}
