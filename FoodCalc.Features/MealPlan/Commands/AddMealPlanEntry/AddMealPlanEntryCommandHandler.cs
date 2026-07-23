using ErrorOr;

using FoodCalc.Features.Mapping;

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
														CancellationToken cancellationToken
	)
	{
		try
		{
			var recipe = await context.Recipes.FirstOrDefaultAsync(r => r.Id == request.RecipeId, cancellationToken);
			if (recipe is null)
				return Error.NotFound(description: "Recipe not found.");

			var dayCount = await context.MealPlanEntries.CountAsync(
				m => m.UserId == request.UserId && m.Date == request.Date, cancellationToken);
			if (dayCount >= MealPlanConstants.MaxRecipesPerDay)
				return Error.Validation(
					description: $"A day can hold at most {MealPlanConstants.MaxRecipesPerDay} recipes.");

			var entry = new MealPlanEntry
			{
				UserId = request.UserId, Date = request.Date, RecipeId = request.RecipeId, Recipe = recipe
			};

			context.MealPlanEntries.Add(entry);
			await context.SaveChangesAsync(cancellationToken);

			return entry.ToDto();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.AddFailed("meal plan entry"));
			return Error.Failure(description: ErrorMessages.Common.AddFailed("meal plan entry"));
		}
	}
}
