using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandHandler(FoodHubDbContext context, ILogger<UpdateRecipeCommandHandler> logger)
    : IRequestHandler<UpdateRecipeCommand, ErrorOr<RecipeDto>>
{
    public async Task<ErrorOr<RecipeDto>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Recipe? recipe =
                await context.Recipes.SingleOrDefaultAsync(r => r.Id == request.Recipe.Id, cancellationToken);

            if (recipe is null)
                return Error.NotFound(description: ErrorMessages.Common.NotFound(ErrorMessages.Entities.Recipe));

            if (!request.Acting.CanEdit(recipe.CreatedByUserId))
                return Error.Forbidden(description: ErrorMessages.Review.NotOwned(ErrorMessages.Entities.Recipe));

            // Every line has to name an existing catalog entry: recipes are searched on that link,
            // so a line without one would be stored but never found. Checked in one query rather
            // than per line, and before anything is assigned, so a rejected request leaves the
            // tracked recipe exactly as it was found.
            var sentIds = request.Recipe.Ingredients.Select(item => item.IngredientId)
                .OfType<Guid>()
                .Distinct()
                .ToList();

            var knownIds = await context.Ingredients.VisibleTo(request.Acting.UserId)
                .Where(i => sentIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync(cancellationToken);

            RecipeItemDto? unlinked =
                request.Recipe.Ingredients.FirstOrDefault(item => item.IngredientId is not {} id || !knownIds.Contains(id));

            if (unlinked is not null)
                return Error.Validation(description: ErrorMessages.Ingredient.UnlinkedLine(unlinked.Name));

            var nameChanged = recipe.Name != request.Recipe.Name;
            recipe.Name = request.Recipe.Name;

            // Reconcile the recipe's items with the set provided in the request:
            // update the ones still present, remove the missing, add the new.
            recipe.Ingredients?.Sync(request.Recipe.Ingredients, keyOfExisting: item => item.Id,
                keyOfIncoming: dto => dto.Id, create: dto => dto.ToEntity(), update: (dto, item) => dto.ApplyTo(item));

            // Ask the change tracker what Sync actually did rather than assuming a request
            // means a change — a save that alters nothing must not cost the author their
            // approval. ApplyTo leaves untouched lines Unmodified, which makes this exact.
            var itemsChanged = context.ChangeTracker.Entries<RecipeItem>()
                .Any(entry => entry.Entity.RecipeId == recipe.Id &&
                              entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

            if (nameChanged || itemsChanged)
                recipe.IsReviewed = false;

            await context.SaveChangesAsync(cancellationToken);

            return recipe.ToDto();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.UpdateFailed(ErrorMessages.Entities.Recipe));
            return Error.Failure(description: ErrorMessages.Common.UpdateFailed(ErrorMessages.Entities.Recipe));
        }
    }
}