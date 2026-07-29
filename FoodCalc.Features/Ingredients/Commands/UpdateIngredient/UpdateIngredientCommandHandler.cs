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
                return Error.NotFound(description: ErrorMessages.Common.NotFound("Ingredient"));

            if (!request.Acting.CanEdit(ingredient.CreatedByUserId))
                return Error.Forbidden(description: ErrorMessages.Review.NotOwned("ingredient"));

            var changed = ingredient.Name != request.Ingredient.Name ||
                          ingredient.ShouldBeAddedToShoppingCart != request.Ingredient.ShouldBeAddedToShoppingCart;

            ingredient.Name = request.Ingredient.Name;
            ingredient.ShouldBeAddedToShoppingCart = request.Ingredient.ShouldBeAddedToShoppingCart;

            // Edited entries go back through approval, but only on a real change — see the
            // matching note in UpdateRecipeNameCommandHandler.
            if (changed)
                ingredient.IsReviewed = false;

            await context.SaveChangesAsync(cancellationToken);

            return ingredient.ToDto();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.UpdateFailed("ingredient"));
            return Error.Failure(description: ErrorMessages.Common.UpdateFailed("ingredient"));
        }
    }
}