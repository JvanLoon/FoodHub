using FoodHub.Persistence.Entities;
using FoodHub.ServiceDefaults;

using Microsoft.AspNetCore.Components;

using System.Net.Http.Json;

namespace FoodCalc.Web.Components.Services
{
    public class RecipeService(HttpClient httpClient, NavigationManager navigationManager)
    {
        public NavigationManager navigationManager { get; set; } = navigationManager;

        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            var response = await httpClient.GetAsync("api/recipe");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Recipe>>() ?? new List<Recipe>();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(Guid recipeId)
        {
            var response = await httpClient.GetAsync($"api/recipe/{recipeId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Recipe>();
        }

        public async Task<Recipe?> AddRecipe(Recipe recipe)
        {
            var response = await httpClient.PostAsJsonAsync("api/recipe", recipe);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Recipe>();
        }

        public async Task<HttpResponseMessage> UpdateRecipe(Recipe recipe)
        {
            var response = await httpClient.PutAsJsonAsync("api/recipe", recipe);
            response.EnsureSuccessStatusCode();
            return response;
        }

		public async Task<HttpResponseMessage> UpdateRecipeName(Guid recipeId, string recipeName)
		{
			var payload = new RecipeNameUpdateDto { Id = recipeId, Name = recipeName };
			var response = await httpClient.PutAsJsonAsync("api/recipe/name", payload);
			response.EnsureSuccessStatusCode();
			return response;
		}

		public async Task<HttpResponseMessage> DeleteRecipe(Guid recipeId)
		{
			var response = await httpClient.DeleteAsync($"api/recipe/deleterecipe/{recipeId}");
			response.EnsureSuccessStatusCode();
			return response;
		}

		public async Task<List<Ingredient>> GetIngredientsAsync()
        {
            var response = await httpClient.GetAsync("api/recipe/ingredients");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Ingredient>>() ?? new List<Ingredient>();
        }

        public async Task<HttpResponseMessage> AddIngredient(RecipeIngredient ingredient)
        {
            var response = await httpClient.PostAsJsonAsync("api/recipe/ingredient", ingredient);
            response.EnsureSuccessStatusCode();
            return response;
        }

		public async Task<HttpResponseMessage> DeleteIngredient(Guid recipeIngredientId)
		{
			var response = await httpClient.DeleteAsync($"api/recipe/deleteingredient/{recipeIngredientId}");
			response.EnsureSuccessStatusCode();
			return response;
		}
	}
}
