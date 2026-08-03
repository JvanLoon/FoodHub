using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;

public class AddIngredientToRecipeCommandHandler(
    FoodHubDbContext context,
    ILogger<AddIngredientToRecipeCommandHandler> logger)
    : IRequestHandler<AddIngredientToRecipeCommand, ErrorOr<RecipeItemDto>>
{
    public async Task<ErrorOr<RecipeItemDto>> Handle(AddIngredientToRecipeCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.RecipeItem;

            var recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == dto.RecipeId, cancellationToken);

            if (recipe is null)
                return Error.NotFound(description: ErrorMessages.Common.NotFound(ErrorMessages.Entities.Recipe));

            if (!request.Acting.CanEdit(recipe.CreatedByUserId))
                return Error.Forbidden(description: ErrorMessages.Review.NotOwned(ErrorMessages.Entities.Recipe));
            
            if (!await context.Ingredients.VisibleTo(request.Acting.UserId)
                    .AnyAsync(i => i.Id == dto.IngredientId, cancellationToken))
            {
                return Error.Validation(description: ErrorMessages.Ingredient.UnlinkedLine(dto.Ingredient.Name));
            }

            var existing = await context.RecipeItems.FirstOrDefaultAsync(
                ri => ri.Id == dto.Id && ri.RecipeId == dto.RecipeId, cancellationToken);

            if (existing != null)
            {
                existing.IngredientId = dto.IngredientId;
                existing.Amount = dto.Amount;
                existing.IngredientAmount = (IngredientAmountType) dto.IngredientAmount;
                existing.ShouldBeAddedToShoppingCart = dto.ShouldBeAddedToShoppingCart;

                // Changing one line sends the whole recipe back for approval, and marks this
                // line so the review screen can point the moderator straight at it.
                existing.IsReviewed = false;
                recipe.IsReviewed = false;

                await context.SaveChangesAsync(cancellationToken);
                return existing.ToDto();
            }

            RecipeItem mappedRecipeItem = dto.ToEntity();
            mappedRecipeItem.IngredientId = dto.IngredientId;
            mappedRecipeItem.IsReviewed = false;
            recipe.IsReviewed = false;

            context.RecipeItems.Add(mappedRecipeItem);
            await context.SaveChangesAsync(cancellationToken);
            return mappedRecipeItem.ToDto();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Recipe.AddIngredientFailed);
            return Error.Failure(description: ErrorMessages.Recipe.UpdateForIngredientFailed);
        }
    }
}