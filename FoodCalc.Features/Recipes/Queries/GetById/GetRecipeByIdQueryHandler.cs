using ErrorOr;
using AutoMapper;

using FoodHub.DTOs;

using MediatR;

using Microsoft.Extensions.Logging;
using FoodHub.Persistence.Repositories;

namespace FoodCalc.Features.Recipes.Queries.GetById;
public class GetRecipeByIdQueryHandler(RecipeRepository recipeRepository, IMapper mapper, ILogger<GetRecipeByIdQueryHandler> logger) : IRequestHandler<GetRecipeByIdQuery, ErrorOr<RecipeDto?>>
{
	public async Task<ErrorOr<RecipeDto?>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var recipe = await recipeRepository.GetByIdAsync(request.Id, cancellationToken);

			if (recipe is null)
			{
				return Error.Failure("Recipe not found");
			}
			return mapper.Map<RecipeDto>(recipe);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, $"Failed to get recipe by id: {request.Id}");
			return Error.Failure($"Failed to get recipe by id: {request.Id}");
		}
	}
}
