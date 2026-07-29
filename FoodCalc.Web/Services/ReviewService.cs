using FoodCalc.Web.Services.Auth;
using FoodHub.DTOs;

namespace FoodCalc.Web.Services;

/// <summary>
/// Client for the moderation queue. Every call here is Admin/Moderator-only on the server; the
/// UI hides the tab from everyone else, but the API is what actually enforces it. Approve
/// publishes the target; reject deletes it — no reason, nothing recorded. The recipe is the
/// gate: approving a recipe approves its ingredients too, so there is no ingredient call here.
/// </summary>
public class ReviewService(AuthenticatedHttpClientService httpClient)
{
    public Task<ApiResult<ReviewQueueDto>> GetQueueAsync() =>
        httpClient.GetAsync<ReviewQueueDto>(ApiRoutes.Review.Queue);

    public Task<ApiResult> ApproveRecipeAsync(Guid recipeId) => Post(ApiRoutes.Review.ApproveRecipe, recipeId);

    public Task<ApiResult> RejectRecipeAsync(Guid recipeId) => Post(ApiRoutes.Review.RejectRecipe, recipeId);

    public Task<ApiResult> ApproveRecipeItemAsync(Guid recipeItemId) =>
        Post(ApiRoutes.Review.ApproveRecipeItem, recipeItemId);

    public Task<ApiResult> RejectRecipeItemAsync(Guid recipeItemId) =>
        Post(ApiRoutes.Review.RejectRecipeItem, recipeItemId);

    private Task<ApiResult> Post(string route, Guid id) => httpClient.PostAsync(route, new ReviewTargetDto { Id = id });
}
