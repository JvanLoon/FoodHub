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
                        return Error.NotFound(description: ErrorMessages.Common.NotFound("Recipe"));

                    // Its lines cascade with it (see RecipeConfiguration).
                    context.Recipes.Remove(recipe);
                    break;
                }
                case ReviewTargetType.RecipeItem:
                {
                    var item = await context.RecipeItems.SingleOrDefaultAsync(ri => ri.Id == request.TargetId,
                        cancellationToken);
                    if (item is null)
                        return Error.NotFound(description: ErrorMessages.Common.NotFound("Recipe line"));

                    context.RecipeItems.Remove(item);
                    break;
                }
                default:
                    return Error.Validation(description: $"Unknown review target type: {request.TargetType}.");
            }

            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.DeleteFailed("rejected item"));
            return Error.Failure(description: ErrorMessages.Common.DeleteFailed("rejected item"));
        }
    }
}
