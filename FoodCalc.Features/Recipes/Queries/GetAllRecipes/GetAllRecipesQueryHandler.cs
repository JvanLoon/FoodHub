using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public class GetAllRecipesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllRecipesQueryHandler> logger) : IRequestHandler<GetAllRecipesQuery, ErrorOr<List<Recipe>>>
{
	public async Task<ErrorOr<List<Recipe>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
	{
		try
		{
			return await unitOfWork.RecipeRepository.GetAllAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all Recipes");
			return Error.Failure("Failed to get all Recipes");
		}
	}
}
