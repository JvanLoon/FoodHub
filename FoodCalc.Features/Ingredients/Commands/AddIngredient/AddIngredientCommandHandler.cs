using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredients.Commands.AddIngredient
{
    public class AddIngredientCommandHandler(FoodHubDbContext context, ILogger<AddIngredientCommandHandler> logger)
        : IRequestHandler<AddIngredientCommand, ErrorOr<IngredientDto>>
    {
        public async Task<ErrorOr<IngredientDto>> Handle(AddIngredientCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.CreatedByUserId))
            {
                return Error.Validation(description: ErrorMessages.Review.NoUser);
            }

            try
            {
                Ingredient ingredient = request.Ingredient.ToEntity();
                ingredient.CreatedByUserId = request.CreatedByUserId;

                // Unapproved on creation for everyone, admins included — same rule as recipes.
                ingredient.IsReviewed = false;

                context.Ingredients.Add(ingredient);
                await context.SaveChangesAsync(cancellationToken);

                return ingredient.ToDto();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.Common.AddFailed(ErrorMessages.Entities.Ingredient));
                return Error.Failure(description: ErrorMessages.Common.AddFailed(ErrorMessages.Entities.Ingredient));
            }
        }
    }
}