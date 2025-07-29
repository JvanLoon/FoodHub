using ErrorOr;
using MediatR;
using AutoMapper;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public class GetAllRecipesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllRecipesQueryHandler> logger) : IRequestHandler<GetAllRecipesQuery, ErrorOr<List<RecipeDto>>>
{
	public async Task<ErrorOr<List<RecipeDto>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var recipes = await unitOfWork.RecipeRepository.GetAllAsync(cancellationToken);
			return mapper.Map<List<RecipeDto>>(recipes);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all Recipes");
			return Error.Failure("Failed to get all Recipes");
		}
	}
}
