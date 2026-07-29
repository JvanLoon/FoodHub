using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipeName;

public class UpdateRecipeNameCommandHandler(FoodHubDbContext context, ILogger<UpdateRecipeNameCommandHandler> logger)
    : IRequestHandler<UpdateRecipeNameCommand, ErrorOr<RecipeDto>>
{
    public async Task<ErrorOr<RecipeDto>> Handle(UpdateRecipeNameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Recipe? recipe =
                await context.Recipes.SingleOrDefaultAsync(r => r.Id == request.RecipeId, cancellationToken);

            if (recipe is null)
                return Error.NotFound(description: ErrorMessages.Common.NotFound("Recipe"));

            if (!request.Acting.CanEdit(recipe.CreatedByUserId))
                return Error.Forbidden(description: ErrorMessages.Review.NotOwned("recipe"));

            // Compare before assigning: saving the form without touching the name must not
            // pull an approved recipe back into the review queue for no reason.
            if (!string.IsNullOrWhiteSpace(request.newRecipeName) && request.newRecipeName != recipe.Name)
            {
                recipe.Name = request.newRecipeName;

                // A rename republishes the recipe under a new title, so it needs approval again.
                recipe.IsReviewed = false;
            }

            await context.SaveChangesAsync(cancellationToken);

            return recipe.ToDto();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.UpdateFailed("recipe"));
            return Error.Failure(description: ErrorMessages.Common.UpdateFailed("recipe"));
        }
    }
}