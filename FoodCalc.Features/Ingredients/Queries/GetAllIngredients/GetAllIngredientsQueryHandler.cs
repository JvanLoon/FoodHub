using ErrorOr;
using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;

public class GetAllIngredientsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllIngredientsQueryHandler> logger) : IRequestHandler<GetAllIngredientsQuery, ErrorOr<List<IngredientDto>>>
{
	public async Task<ErrorOr<List<IngredientDto>>> Handle(GetAllIngredientsQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var ingredients = await unitOfWork.IngredientRepository.GetAllAsync(cancellationToken);
			return mapper.Map<List<IngredientDto>>(ingredients);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all ingredients");
			return Error.Failure("Failed to get all ingredients");
		}
	}
}
