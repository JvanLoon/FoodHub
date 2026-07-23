using ErrorOr;

using FoodCalc.Features.Mapping;

using FoodHub.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Queries.GetById;

public class GetRecipeByIdQueryHandler(FoodHubDbContext context, ILogger<GetRecipeByIdQueryHandler> logger)
	: IRequestHandler<GetRecipeByIdQuery, ErrorOr<RecipeDto?>>
{
	public async Task<ErrorOr<RecipeDto?>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

			if (recipe is null) { return Error.Failure(description: ErrorMessages.Common.NotFound("Recipe")); }

			return recipe.ToDto();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, $"Failed to get recipe by id: {request.Id}");
			return Error.Failure(description: $"Failed to get recipe by id: {request.Id}");
		}
	}
}
