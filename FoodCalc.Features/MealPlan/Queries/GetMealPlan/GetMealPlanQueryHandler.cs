using ErrorOr;

using FoodCalc.Features.Mapping;

using FoodHub.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.MealPlan.Queries.GetMealPlan;

public class GetMealPlanQueryHandler(FoodHubDbContext context, ILogger<GetMealPlanQueryHandler> logger)
	: IRequestHandler<GetMealPlanQuery, ErrorOr<List<MealPlanEntryDto>>>
{
	public async Task<ErrorOr<List<MealPlanEntryDto>>> Handle(GetMealPlanQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var entries = await context.MealPlanEntries
				.Where(m => m.UserId == request.UserId && m.Date >= request.From && m.Date <= request.To)
				.Include(m => m.Recipe)
				.OrderBy(m => m.Date)
				.ThenBy(m => m.CreatedDate)
				.ToListAsync(cancellationToken);

			return entries.ToDtoList();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.GetAllFailed("MealPlan"));
			return Error.Failure(description: ErrorMessages.Common.GetAllFailed("MealPlan"));
		}
	}
}
