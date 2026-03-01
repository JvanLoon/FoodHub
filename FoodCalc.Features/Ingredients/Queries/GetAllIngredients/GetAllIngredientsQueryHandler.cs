using ErrorOr;
using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

using FoodCalc.Features;

namespace FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;

public class GetAllIngredientsQueryHandler(UnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllIngredientsQueryHandler> logger) : IRequestHandler<GetAllIngredientsQuery, ErrorOr<PagedResultDto<IngredientDto>>>
{
	public async Task<ErrorOr<PagedResultDto<IngredientDto>>> Handle(GetAllIngredientsQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var query = unitOfWork.IngredientRepository.GetAllAsync();

			if (!string.IsNullOrWhiteSpace(request.Search))
				query = query.Where(i => i.Name.Contains(request.Search));

			var paged = await query.ToPagedResultAsync(request.Page, request.PageSize, cancellationToken);

			return new PagedResultDto<IngredientDto>
			{
				Items = mapper.Map<List<IngredientDto>>(paged.Items),
				TotalCount = paged.TotalCount,
				Page = paged.Page,
				PageSize = paged.PageSize
			};
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all ingredients");
			return Error.Failure("Failed to get all ingredients");
		}
	}
}
