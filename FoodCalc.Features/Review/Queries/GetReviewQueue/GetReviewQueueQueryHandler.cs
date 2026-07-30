using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
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
            // its lines — which is what the review screen shows, changed ones marked. The recipe
            // is the only review gate; ingredients are approved with the recipe that uses them.
            var recipes = await context.Recipes.Where(r => !r.IsReviewed)
                .OrderBy(r => r.ModifiedDate)
                .ToListAsync(cancellationToken);

            if (recipes.Count == 0)
                return new ReviewQueueDto();

            var emails = await ResolveEmailsAsync(recipes.Select(r => r.CreatedByUserId));

            return new ReviewQueueDto
            {
                Recipes = [..recipes.Select(r => r.ToPendingDto(DisplayName(r.CreatedByUserId, emails)))]
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.GetAllFailed(ErrorMessages.Entities.ReviewQueue));
            return Error.Failure(description: ErrorMessages.Common.GetAllFailed(ErrorMessages.Entities.ReviewQueue));
        }
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
}
