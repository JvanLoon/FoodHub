using ErrorOr;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Feature.Ingredient.Queries.GetAllIngredients;

public class GetAllIngredientsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllIngredientsQueryHandler> logger) : IRequestHandler<GetAllIngredientsQuery, ErrorOr<List<FoodHub.Persistence.Entities.Ingredient>>>
{
	public async Task<ErrorOr<List<FoodHub.Persistence.Entities.Ingredient>>> Handle(GetAllIngredientsQuery request, CancellationToken cancellationToken)
	{
		try
		{
			return await unitOfWork.IngredientRepository.GetAllAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all ingredients");
			return Error.Failure("Failed to get all ingredients");
		}
	}
}
