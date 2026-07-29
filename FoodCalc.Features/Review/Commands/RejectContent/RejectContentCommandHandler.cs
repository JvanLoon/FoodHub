using ErrorOr;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Review.Commands.RejectContent;

/// <summary>
/// Records a moderator's rejection and, if asked, deletes what was rejected.
///
/// The <see cref="ReviewRejection"/> row is written whether or not the target survives — it is
/// the only place the reason lives, and the planned notification feature reads it to tell the
/// author why. A rejection that keeps the target leaves it unapproved, so it stays visible to
/// its author (who can fix and resubmit) and invisible to everyone else.
/// </summary>
public class RejectContentCommandHandler(FoodHubDbContext context, ILogger<RejectContentCommandHandler> logger)
    : IRequestHandler<RejectContentCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(RejectContentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Error.Validation(description: ErrorMessages.Review.ReasonRequired);

        try
        {
            // Name and owner are read off the target before any delete, because the rejection
            // record has to outlive it.
            string targetName;
            string ownerUserId;

            if (request.TargetType == ReviewTargetType.Recipe)
            {
                var recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == request.TargetId,
                    cancellationToken);

                if (recipe is null)
                    return Error.NotFound(description: ErrorMessages.Common.NotFound("Recipe"));

                targetName = recipe.Name;
                ownerUserId = recipe.CreatedByUserId;

                if (request.Delete)
                    context.Recipes.Remove(recipe);
            }
            else if (request.TargetType == ReviewTargetType.Ingredient)
            {
                var ingredient = await context.Ingredients.SingleOrDefaultAsync(i => i.Id == request.TargetId,
                    cancellationToken);

                if (ingredient is null)
                    return Error.NotFound(description: ErrorMessages.Common.NotFound("Ingredient"));

                targetName = ingredient.Name;
                ownerUserId = ingredient.CreatedByUserId;

                if (request.Delete)
                    context.Ingredients.Remove(ingredient);
            }
            else { return Error.Validation(description: $"Unknown review target type: {request.TargetType}."); }

            context.ReviewRejections.Add(new ReviewRejection
            {
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                TargetName = targetName,
                TargetOwnerUserId = ownerUserId,
                RejectedByUserId = request.RejectedByUserId ?? string.Empty,
                Reason = request.Reason.Trim(),
                TargetDeleted = request.Delete
            });

            // One SaveChanges for the delete and the record together, so a rejection can never
            // destroy content without leaving the reason behind.
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.UpdateFailed("review status"));
            return Error.Failure(description: ErrorMessages.Common.UpdateFailed("review status"));
        }
    }
}
