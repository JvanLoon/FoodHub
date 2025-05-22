using ErrorOr;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories.Interface;

using MediatR;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Queries.GetById;
public class GetRecipeByIdQueryHandler(IRecipeRepository recipeRepository, ILogger<GetRecipeByIdQueryHandler> logger) : IRequestHandler<GetRecipeByIdQuery, ErrorOr<Recipe?>>
{
	public async Task<ErrorOr<Recipe?>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var recipe = await recipeRepository.GetByIdAsync(request.Id, cancellationToken);

			if (recipe is null)
			{
				return Error.Failure("Recipe not found");
			}
			return recipe;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, $"Failed to get recipe by id: {request.Id}");
			return Error.Failure($"Failed to get recipe by id: {request.Id}");
		}
	}
}
