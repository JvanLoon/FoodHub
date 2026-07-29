using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Review.Commands.ApproveContent;

public class ApproveContentCommandHandler(FoodHubDbContext context, ILogger<ApproveContentCommandHandler> logger)
    : IRequestHandler<ApproveContentCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(ApproveContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return request.TargetType switch
            {
                ReviewTargetType.Recipe => await ApproveRecipeAsync(request.TargetId, cancellationToken),
                ReviewTargetType.RecipeItem => await ApproveRecipeItemAsync(request.TargetId, cancellationToken),
                _ => Error.Validation(description: $"Unknown review target type: {request.TargetType}.")
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.UpdateFailed("review status"));
            return Error.Failure(description: ErrorMessages.Common.UpdateFailed("review status"));
        }
    }

    private async Task<ErrorOr<bool>> ApproveRecipeAsync(Guid id, CancellationToken cancellationToken)
    {
        var recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (recipe is null)
            return Error.NotFound(description: ErrorMessages.Common.NotFound("Recipe"));

        var now = DateTime.UtcNow;

        recipe.IsReviewed = true;
        recipe.FirstApprovedDate ??= now;

        // Approving the recipe approves the lines it was approved with. Leaving them flagged
        // would make the next review of this recipe re-highlight edits a moderator already
        // signed off on.
        var lineNames = new List<string>();
        foreach (var item in recipe.Ingredients ?? [])
        {
            item.IsReviewed = true;
            lineNames.Add(item.Name);
        }

        // The recipe is the only review gate: approving it also approves the ingredient records
        // for its lines, so those ingredient names become allowed everywhere (visible in every
        // user's ingredient search) rather than only inside this recipe. Matched by name, since
        // that is how a recipe line and its ingredient record are tied together.
        if (lineNames.Count > 0)
        {
            var pending = await context.Ingredients
                .Where(ing => !ing.IsReviewed && lineNames.Contains(ing.Name))
                .ToListAsync(cancellationToken);

            foreach (var ing in pending)
            {
                ing.IsReviewed = true;
                ing.FirstApprovedDate ??= now;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Clears the "changed" flag on one line. The parent recipe stays pending — approving a
    /// line just resolves it, so the recipe's own Approve button unlocks once every line is
    /// resolved. Whole-recipe approval (above) is what finally publishes it.
    /// </summary>
    private async Task<ErrorOr<bool>> ApproveRecipeItemAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await context.RecipeItems.SingleOrDefaultAsync(ri => ri.Id == id, cancellationToken);

        if (item is null)
            return Error.NotFound(description: ErrorMessages.Common.NotFound("Recipe line"));

        item.IsReviewed = true;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
