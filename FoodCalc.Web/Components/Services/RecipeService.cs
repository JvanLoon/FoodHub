using FoodCalc.Web.Components.Services.Auth;
using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;
public class RecipeService(AuthenticatedHttpClientService httpClient)
{
    public Task<ApiResult<PagedResultDto<RecipeDto>>> GetPagedRecipesAsync(int page, int pageSize, string? search = null, bool withIngredients = true)
    {
        var url = $"api/recipe/getallrecipes?withingredient={withIngredients}&page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        return httpClient.GetAsync<PagedResultDto<RecipeDto>>(url);
    }

    public async Task<ApiResult<List<RecipeDto>>> GetAllRecipesAsync(bool withIngredients = true)
    {
        var paged = await GetPagedRecipesAsync(1, int.MaxValue, withIngredients: withIngredients);
        if (!paged.Success)
            return ApiResult<List<RecipeDto>>.Fail(paged.Error!, paged.StatusCode);

        return ApiResult<List<RecipeDto>>.Ok([.. paged.Data!.Items], paged.StatusCode);
    }

    public Task<ApiResult<RecipeDto>> GetRecipeByIdAsync(Guid recipeId) =>
        httpClient.GetAsync<RecipeDto>($"api/recipe/{recipeId}");

    public Task<ApiResult<RecipeDto>> AddRecipe(CreateRecipeDto recipe) =>
        httpClient.PostAsync<CreateRecipeDto, RecipeDto>("api/recipe", recipe);

    public Task<ApiResult> UpdateRecipe(UpdateRecipeDto recipe) =>
        httpClient.PutAsync("api/recipe", recipe);

    public Task<ApiResult> UpdateRecipeName(Guid recipeId, string recipeName)
    {
        var payload = new FoodHub.DTOs.RecipeNameUpdateDto { Id = recipeId, Name = recipeName };
        return httpClient.PutAsync("api/recipe/name", payload);
    }

    public Task<ApiResult> DeleteRecipe(Guid recipeId) =>
        httpClient.DeleteAsync($"api/recipe/deleterecipe/{recipeId}");

    public Task<ApiResult<List<IngredientDto>>> GetIngredientsAsync() =>
        httpClient.GetAsync<List<IngredientDto>>("api/recipe/ingredients");

    public Task<ApiResult> AddIngredient(RecipeIngredientDto ingredient) =>
        httpClient.PostAsync("api/recipe/ingredient", ingredient);

    public Task<ApiResult> DeleteIngredient(Guid recipeIngredientId) =>
        httpClient.DeleteAsync($"api/recipe/deleteingredient/{recipeIngredientId}");
}
