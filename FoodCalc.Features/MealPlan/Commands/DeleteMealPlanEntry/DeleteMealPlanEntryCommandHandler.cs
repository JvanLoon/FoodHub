using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.MealPlan.Commands.DeleteMealPlanEntry;

public class DeleteMealPlanEntryCommandHandler(
	FoodHubDbContext context,
	ILogger<DeleteMealPlanEntryCommandHandler> logger) : IRequestHandler<DeleteMealPlanEntryCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(DeleteMealPlanEntryCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var entry = await context.MealPlanEntries.FirstOrDefaultAsync(
				m => m.Id == request.Id && m.UserId == request.UserId, cancellationToken);
			if (entry != null)
			{
				context.MealPlanEntries.Remove(entry);
				await context.SaveChangesAsync(cancellationToken);
			}

			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.DeleteFailed("meal plan entry"));
			return Error.Failure(description: ErrorMessages.Common.DeleteFailed("meal plan entry"));
		}
	}
}
