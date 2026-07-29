using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Review.Queries.GetReviewQueue;

public class GetReviewQueueQueryHandler(
    FoodHubDbContext context,
    UserManager<IdentityUser> userManager,
    ILogger<GetReviewQueueQueryHandler> logger) : IRequestHandler<GetReviewQueueQuery, ErrorOr<ReviewQueueDto>>
{
    public async Task<ErrorOr<ReviewQueueDto>> Handle(GetReviewQueueQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Recipe.Ingredients is auto-included, so every pending recipe arrives with all of
            // its lines — which is what the review screen shows, changed ones marked.
            var recipes = await context.Recipes.Where(r => !r.IsReviewed)
                .OrderBy(r => r.ModifiedDate)
                .ToListAsync(cancellationToken);

            var ingredients = await context.Ingredients.Where(i => !i.IsReviewed)
                .OrderBy(i => i.CreatedDate)
                .ToListAsync(cancellationToken);

            if (recipes.Count == 0 && ingredients.Count == 0)
                return new ReviewQueueDto();

            var emails = await ResolveEmailsAsync(recipes.Select(r => r.CreatedByUserId)
                .Concat(ingredients.Select(i => i.CreatedByUserId)));

            var rejections = await LoadLatestRejectionsAsync(recipes, ingredients, emails, cancellationToken);

            var queue = new ReviewQueueDto
            {
                Recipes = [..recipes.Select(r => BuildRecipe(r, emails, rejections))],
                Ingredients = [..ingredients.Select(i => BuildIngredient(i, emails, rejections))]
            };

            return queue;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.GetAllFailed("the review queue"));
            return Error.Failure(description: ErrorMessages.Common.GetAllFailed("the review queue"));
        }
    }

    private PendingRecipeDto BuildRecipe(Recipe recipe,
        IReadOnlyDictionary<string, string> emails,
        IReadOnlyDictionary<(ReviewTargetType, Guid), ReviewRejectionDto> rejections)
    {
        var dto = recipe.ToPendingDto(DisplayName(recipe.CreatedByUserId, emails));
        dto.LastRejection = rejections.GetValueOrDefault((ReviewTargetType.Recipe, recipe.Id));
        return dto;
    }

    private PendingIngredientDto BuildIngredient(Ingredient ingredient,
        IReadOnlyDictionary<string, string> emails,
        IReadOnlyDictionary<(ReviewTargetType, Guid), ReviewRejectionDto> rejections)
    {
        var dto = ingredient.ToPendingDto(DisplayName(ingredient.CreatedByUserId, emails));
        dto.LastRejection = rejections.GetValueOrDefault((ReviewTargetType.Ingredient, ingredient.Id));
        return dto;
    }

    /// <summary>
    /// Maps IdentityUser ids to emails in one round trip. Ids with no account (deleted user, or
    /// the empty author on rows that predate review) are simply absent — see <see cref="DisplayName"/>.
    /// </summary>
    private async Task<Dictionary<string, string>> ResolveEmailsAsync(IEnumerable<string> userIds)
    {
        var ids = userIds.Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();

        if (ids.Count == 0)
            return [];

        return await userManager.Users.Where(u => ids.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? u.UserName ?? u.Id);
    }

    /// <summary>
    /// Falls back to the raw id rather than an empty cell: a moderator looking at an
    /// unattributable submission should see that there is an author they cannot resolve.
    /// </summary>
    private static string DisplayName(string userId, IReadOnlyDictionary<string, string> emails) =>
        string.IsNullOrEmpty(userId) ? "(unknown)" : emails.GetValueOrDefault(userId, userId);

    /// <summary>
    /// The most recent rejection per pending item, so the queue can distinguish a resubmission
    /// from a first submission — otherwise a rejected-but-kept item is indistinguishable from a
    /// new one and gets re-judged from scratch every time.
    /// </summary>
    private async Task<Dictionary<(ReviewTargetType, Guid), ReviewRejectionDto>> LoadLatestRejectionsAsync(
        List<Recipe> recipes,
        List<Ingredient> ingredients,
        Dictionary<string, string> emails,
        CancellationToken cancellationToken)
    {
        var recipeIds = recipes.Select(r => r.Id)
            .ToList();
        var ingredientIds = ingredients.Select(i => i.Id)
            .ToList();

        var rows = await context.ReviewRejections
            .Where(r => (r.TargetType == ReviewTargetType.Recipe && recipeIds.Contains(r.TargetId)) ||
                        (r.TargetType == ReviewTargetType.Ingredient && ingredientIds.Contains(r.TargetId)))
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
            return [];

        // Rejecting moderators are usually a handful of accounts the caller's own map does not
        // cover, so resolve them too before projecting.
        foreach (var (id, email) in await ResolveEmailsAsync(rows.Select(r => r.RejectedByUserId)))
        {
            emails[id] = email;
        }

        return rows.GroupBy(r => (r.TargetType, r.TargetId))
            .Select(g => (g.Key, Latest: g.MaxBy(r => r.CreatedDate)!))
            .ToDictionary(x => x.Key, x => x.Latest.ToDto(DisplayName(x.Latest.RejectedByUserId, emails)));
    }
}
