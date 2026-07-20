using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

using FoodCalc.Features;

namespace FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;

public class GetAllIngredientsQueryHandler(FoodHubDbContext context, ILogger<GetAllIngredientsQueryHandler> logger) : IRequestHandler<GetAllIngredientsQuery, ErrorOr<PagedResultDto<IngredientDto>>>
{
	public async Task<ErrorOr<PagedResultDto<IngredientDto>>> Handle(GetAllIngredientsQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var query = context.Ingredients.AsQueryable();

			if (!string.IsNullOrWhiteSpace(request.Search))
				query = query.Where(i => i.Name.Contains(request.Search));

			return await query.ToPagedResultAsync(request, items => items.ToDtoList(), cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.GetAllFailed("ingredients"));
			return Error.Failure(ErrorMessages.Common.GetAllFailed("ingredients"));
		}
	}
}
