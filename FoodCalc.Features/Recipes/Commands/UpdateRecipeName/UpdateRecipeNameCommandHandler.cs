using ErrorOr;

using FoodCalc.Features.Mapping;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipeName;

public class UpdateRecipeNameCommandHandler(FoodHubDbContext context, ILogger<UpdateRecipeNameCommandHandler> logger) : IRequestHandler<UpdateRecipeNameCommand, ErrorOr<RecipeDto>>
{
	public async Task<ErrorOr<RecipeDto>> Handle(UpdateRecipeNameCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Recipe recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == request.RecipeId, cancellationToken) ??
							throw new Exception($"recipe by id:{request.RecipeId} not found.");

			if (!string.IsNullOrWhiteSpace(request.newRecipeName))
			{
				recipe.Name = request.newRecipeName;
			}

			await context.SaveChangesAsync(cancellationToken);

			return recipe.ToDto();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.UpdateFailed("recipe"));
			return Error.Failure(description: ErrorMessages.Common.UpdateFailed("recipe"));
		}
	}
}
