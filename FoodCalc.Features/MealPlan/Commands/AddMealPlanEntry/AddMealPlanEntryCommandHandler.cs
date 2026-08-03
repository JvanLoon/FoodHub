using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.MealPlan.Commands.AddMealPlanEntry;

public class AddMealPlanEntryCommandHandler(FoodHubDbContext context, ILogger<AddMealPlanEntryCommandHandler> logger)
    : IRequestHandler<AddMealPlanEntryCommand, ErrorOr<MealPlanEntryDto>>
{
    public async Task<ErrorOr<MealPlanEntryDto>> Handle(AddMealPlanEntryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Planning a recipe requires being able to see it, so an unapproved recipe
            // belonging to someone else is "not found" here just as it is on the read path.
            var recipe = await context.Recipes.VisibleTo(request.UserId)
                .FirstOrDefaultAsync(r => r.Id == request.RecipeId, cancellationToken);
            
            if (recipe is null)
                return Error.NotFound(description: ErrorMessages.Common.NotFound(ErrorMessages.Entities.Recipe));

            var dayCount = await context.MealPlanEntries.CountAsync(
                m => m.UserId == request.UserId && m.Date == request.Date, cancellationToken);
            
            if (dayCount >= MealPlanConstants.MaxRecipesPerDay)
                return Error.Validation(
                    description: ErrorMessages.MealPlan.MaxPerDay(MealPlanConstants.MaxRecipesPerDay));

            var entry = new MealPlanEntry
            {
                UserId = request.UserId,
                Date = request.Date,
                RecipeId = request.RecipeId,
                Recipe = recipe
            };

            context.MealPlanEntries.Add(entry);
            await context.SaveChangesAsync(cancellationToken);

            return entry.ToDto();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.AddFailed(ErrorMessages.Entities.MealPlanEntry));
            return Error.Failure(description: ErrorMessages.Common.AddFailed(ErrorMessages.Entities.MealPlanEntry));
        }
    }
}