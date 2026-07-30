using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredients.Commands.UpdateIngredient;

public class UpdateIngredientCommandHandler(FoodHubDbContext context, ILogger<UpdateIngredientCommandHandler> logger)
    : IRequestHandler<UpdateIngredientCommand, ErrorOr<IngredientDto>>
{
    public async Task<ErrorOr<IngredientDto>> Handle(UpdateIngredientCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            Ingredient? ingredient =
                await context.Ingredients.SingleOrDefaultAsync(i => i.Id == request.Ingredient.Id, cancellationToken);

            if (ingredient is null)
                return Error.NotFound(description: ErrorMessages.Common.NotFound(ErrorMessages.Entities.Ingredient));

            if (!request.Acting.CanEdit(ingredient.CreatedByUserId))
                return Error.Forbidden(description: ErrorMessages.Review.NotOwned(ErrorMessages.Entities.Ingredient));

            ingredient.Name = request.Ingredient.Name;
            ingredient.ShouldBeAddedToShoppingCart = request.Ingredient.ShouldBeAddedToShoppingCart;

            // Ingredient edits do NOT reset approval. Ingredients have no review queue of their
            // own (the recipe is the gate), so un-approving one here would strand it — invisible
            // to everyone with no way back. The /ingredients page is staff-only anyway.

            await context.SaveChangesAsync(cancellationToken);

            return ingredient.ToDto();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.UpdateFailed(ErrorMessages.Entities.Ingredient));
            return Error.Failure(description: ErrorMessages.Common.UpdateFailed(ErrorMessages.Entities.Ingredient));
        }
    }
}