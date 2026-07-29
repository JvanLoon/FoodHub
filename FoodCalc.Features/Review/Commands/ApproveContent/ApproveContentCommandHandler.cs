using ErrorOr;
using FoodHub.Persistence.Entities;
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
                ReviewTargetType.Ingredient => await ApproveIngredientAsync(request.TargetId, cancellationToken),
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

        recipe.IsReviewed = true;
        recipe.FirstApprovedDate ??= DateTime.UtcNow;

        // Approving the recipe approves the lines it was approved with. Leaving them flagged
        // would make the next review of this recipe re-highlight edits a moderator already
        // signed off on.
        foreach (var item in recipe.Ingredients ?? []) { item.IsReviewed = true; }

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<ErrorOr<bool>> ApproveIngredientAsync(Guid id, CancellationToken cancellationToken)
    {
        var ingredient = await context.Ingredients.SingleOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (ingredient is null)
            return Error.NotFound(description: ErrorMessages.Common.NotFound("Ingredient"));

        ingredient.IsReviewed = true;
        ingredient.FirstApprovedDate ??= DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
