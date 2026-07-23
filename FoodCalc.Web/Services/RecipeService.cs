using FoodCalc.Web.Services.Auth;

using FoodHub.DTOs;

namespace FoodCalc.Web.Services;

public class RecipeService(AuthenticatedHttpClientService httpClient)
{
	public Task<ApiResult<PagedResultDto<RecipeDto>>> GetPagedRecipesAsync(int page, int pageSize,
		string? search = null, bool withIngredients = true)
	{
		var url = $"{ApiRoutes.Recipe.GetAll}?withingredient={withIngredients}&page={page}&pageSize={pageSize}";
		if (!string.IsNullOrWhiteSpace(search))
			url += $"&search={Uri.EscapeDataString(search)}";

		return httpClient.GetAsync<PagedResultDto<RecipeDto>>(url);
	}

	public async Task<ApiResult<List<RecipeDto>>> GetAllRecipesAsync(bool withIngredients = true)
	{
		var paged = await GetPagedRecipesAsync(1, int.MaxValue, withIngredients: withIngredients);
		if (!paged.Success)
			return ApiResult<List<RecipeDto>>.Fail(paged.Errors, paged.StatusCode);

		return ApiResult<List<RecipeDto>>.Ok([.. paged.Data!.Items], paged.StatusCode);
	}

	public Task<ApiResult<RecipeDto>> GetRecipeByIdAsync(Guid recipeId) =>
		httpClient.GetAsync<RecipeDto>(ApiRoutes.Recipe.GetById(recipeId));

	public Task<ApiResult<RecipeDto>> AddRecipe(CreateRecipeDto recipe) =>
		httpClient.PostAsync<CreateRecipeDto, RecipeDto>(ApiRoutes.Recipe.Create, recipe);

	public Task<ApiResult> UpdateRecipe(UpdateRecipeDto recipe) =>
		httpClient.PutAsync(ApiRoutes.Recipe.Update, recipe);

	public Task<ApiResult> UpdateRecipeName(Guid recipeId, string recipeName)
	{
		var payload = new FoodHub.DTOs.RecipeNameUpdateDto {Id = recipeId, Name = recipeName};
		return httpClient.PutAsync(ApiRoutes.Recipe.UpdateName, payload);
	}

	public Task<ApiResult> DeleteRecipe(Guid recipeId) =>
		httpClient.DeleteAsync(ApiRoutes.Recipe.DeleteRecipe(recipeId));

	// NOTE: no matching API endpoint exists for this route and the method is currently unused.
	public Task<ApiResult<List<IngredientDto>>> GetIngredientsAsync() =>
		httpClient.GetAsync<List<IngredientDto>>("api/recipe/ingredients");

	public Task<ApiResult> AddIngredient(RecipeItemDto ingredient) =>
		httpClient.PostAsync(ApiRoutes.Recipe.AddIngredient, ingredient);

	public Task<ApiResult> DeleteIngredient(Guid recipeItemId) =>
		httpClient.DeleteAsync(ApiRoutes.Recipe.DeleteIngredient(recipeItemId));
}
