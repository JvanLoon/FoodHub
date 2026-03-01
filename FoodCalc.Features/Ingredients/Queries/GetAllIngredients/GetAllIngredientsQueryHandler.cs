using ErrorOr;
using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

using FoodCalc.Features;

namespace FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;

public class GetAllIngredientsQueryHandler(UnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllIngredientsQueryHandler> logger) : IRequestHandler<GetAllIngredientsQuery, ErrorOr<List<IngredientDto>>>
{
	public async Task<ErrorOr<List<IngredientDto>>> Handle(GetAllIngredientsQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var ingredients = await unitOfWork.IngredientRepository.GetAllAsync().ToPagedResultAsync(request.Page, request.PageSize);

			return mapper.Map<List<IngredientDto>>(ingredients.Items);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all ingredients");
			return Error.Failure("Failed to get all ingredients");
		}
	}
}
