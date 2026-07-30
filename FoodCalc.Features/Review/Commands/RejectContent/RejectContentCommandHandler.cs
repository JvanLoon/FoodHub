using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Review.Commands.RejectContent;

/// <summary>
/// Rejecting means deleting. A rejected recipe is removed outright; a rejected recipe line is
/// removed from its recipe, which stays pending until the moderator resolves its remaining
/// lines and approves it. Nothing about the rejection is kept. Ingredients have no independent
/// review, so there is no ingredient reject — the recipe is the only gate.
///
/// When removing a line (or a whole recipe's lines) leaves an ingredient that no other recipe
/// uses, that ingredient record is deleted too: a one-off ingredient added for a rejected
/// recipe should not linger in everyone's ingredient search. An ingredient still used by
/// another recipe is left alone.
/// </summary>
public class RejectContentCommandHandler(FoodHubDbContext context, ILogger<RejectContentCommandHandler> logger)
    : IRequestHandler<RejectContentCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(RejectContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            switch (request.TargetType)
            {
                case ReviewTargetType.Recipe:
                {
                    var recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == request.TargetId,
                        cancellationToken);
                    if (recipe is null)
                        return Error.NotFound(description: ErrorMessages.Common.NotFound(ErrorMessages.Entities.Recipe));

                    // Its lines cascade with it (see RecipeConfiguration). Any ingredient those
                    // lines introduced and no other recipe uses is now an orphan — remove it.
                    var names = (recipe.Ingredients ?? []).Select(i => i.Name)
                        .ToList();

                    context.Recipes.Remove(recipe);

                    await RemoveOrphanIngredientsAsync(names,
                        name => context.RecipeItems.AnyAsync(ri => ri.RecipeId != recipe.Id && ri.Name == name,
                            cancellationToken), cancellationToken);
                    break;
                }
                case ReviewTargetType.RecipeItem:
                {
                    var item = await context.RecipeItems.SingleOrDefaultAsync(ri => ri.Id == request.TargetId,
                        cancellationToken);
                    if (item is null)
                        return Error.NotFound(description: ErrorMessages.Common.NotFound(ErrorMessages.Entities.RecipeLine));

                    context.RecipeItems.Remove(item);

                    // If nothing else uses this ingredient, drop the ingredient record too.
                    await RemoveOrphanIngredientsAsync([item.Name],
                        name => context.RecipeItems.AnyAsync(ri => ri.Id != item.Id && ri.Name == name,
                            cancellationToken), cancellationToken);
                    break;
                }
                default:
                    return Error.Validation(description: ErrorMessages.Review.UnknownTargetType(request.TargetType));
            }

            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.DeleteFailed(ErrorMessages.Entities.RejectedItem));
            return Error.Failure(description: ErrorMessages.Common.DeleteFailed(ErrorMessages.Entities.RejectedItem));
        }
    }

    /// <summary>
    /// Deletes the ingredient record(s) for each of <paramref name="names"/> that
    /// <paramref name="stillUsed"/> reports is no longer referenced by any recipe line. The
    /// "still used" check is passed in because the exclusion differs (a single rejected line vs.
    /// a whole recipe's lines) and it runs against the database, which still holds the rows being
    /// removed until <c>SaveChanges</c>.
    /// </summary>
    private async Task RemoveOrphanIngredientsAsync(IEnumerable<string> names,
        Func<string, Task<bool>> stillUsed,
        CancellationToken cancellationToken)
    {
        foreach (var name in names.Distinct())
        {
            if (await stillUsed(name))
                continue;

            var orphans = await context.Ingredients.Where(i => i.Name == name)
                .ToListAsync(cancellationToken);

            context.Ingredients.RemoveRange(orphans);
        }
    }
}
