using FoodCalc.Web.Services.Auth;
using FoodHub.DTOs;

namespace FoodCalc.Web.Services;

/// <summary>
/// Client for the moderation queue. Every call here is Admin/Moderator-only on the server; the
/// UI hides the tab from everyone else, but the API is what actually enforces it.
/// </summary>
public class ReviewService(AuthenticatedHttpClientService httpClient)
{
    public Task<ApiResult<ReviewQueueDto>> GetQueueAsync() =>
        httpClient.GetAsync<ReviewQueueDto>(ApiRoutes.Review.Queue);

    public Task<ApiResult> ApproveRecipeAsync(Guid recipeId) => httpClient.PostAsync(ApiRoutes.Review.ApproveRecipe,
        new ApproveReviewDto
        {
            Id = recipeId
        });

    public Task<ApiResult> ApproveIngredientAsync(Guid ingredientId) => httpClient.PostAsync(
        ApiRoutes.Review.ApproveIngredient, new ApproveReviewDto
        {
            Id = ingredientId
        });

    public Task<ApiResult> RejectRecipeAsync(Guid recipeId, string reason, bool delete) => httpClient.PostAsync(
        ApiRoutes.Review.RejectRecipe, new RejectReviewDto
        {
            Id = recipeId,
            Reason = reason,
            Delete = delete
        });

    public Task<ApiResult> RejectIngredientAsync(Guid ingredientId, string reason, bool delete) =>
        httpClient.PostAsync(ApiRoutes.Review.RejectIngredient, new RejectReviewDto
        {
            Id = ingredientId,
            Reason = reason,
            Delete = delete
        });
}
