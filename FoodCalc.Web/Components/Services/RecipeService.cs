using FoodCalc.Web.Components.Services.Auth;
using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;
public class RecipeService(AuthenticatedHttpClientService httpClient)
{
    public async Task<PagedResultDto<RecipeDto>> GetPagedRecipesAsync(int page, int pageSize, string? search = null, bool withIngredients = true)
    {
        var url = $"api/recipe/getallrecipes?withingredient={withIngredients}&page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResultDto<RecipeDto>>() ?? new();
    }

    public async Task<List<RecipeDto>> GetAllRecipesAsync(bool withIngredients = true)
    {
        var paged = await GetPagedRecipesAsync(1, int.MaxValue, withIngredients: withIngredients);
        return [..paged.Items];
    }

	public async Task<RecipeDto?> GetRecipeByIdAsync(Guid recipeId)
    {
        var response = await httpClient.GetAsync($"api/recipe/{recipeId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RecipeDto>();
    }

    public async Task<RecipeDto?> AddRecipe(CreateRecipeDto recipe)
    {
        var response = await httpClient.PostAsJsonAsync("api/recipe", recipe);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RecipeDto>();
    }

    public async Task<HttpResponseMessage> UpdateRecipe(UpdateRecipeDto recipe)
    {
        var response = await httpClient.PutAsJsonAsync("api/recipe", recipe);
        response.EnsureSuccessStatusCode();
        return response;
    }

    public async Task<HttpResponseMessage> UpdateRecipeName(Guid recipeId, string recipeName)
    {
        var payload = new FoodHub.DTOs.RecipeNameUpdateDto { Id = recipeId, Name = recipeName };
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

    public async Task<List<IngredientDto>> GetIngredientsAsync()
    {
        var response = await httpClient.GetAsync("api/recipe/ingredients");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<IngredientDto>>() ?? new List<IngredientDto>();
    }

    public async Task<HttpResponseMessage> AddIngredient(RecipeIngredientDto ingredient)
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
