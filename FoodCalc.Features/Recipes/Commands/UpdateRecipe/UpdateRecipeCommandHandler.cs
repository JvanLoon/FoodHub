using ErrorOr;

using FoodCalc.Features.Mapping;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Extensions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandHandler(FoodHubDbContext context, ILogger<UpdateRecipeCommandHandler> logger)
	: IRequestHandler<UpdateRecipeCommand, ErrorOr<RecipeDto>>
{
	public async Task<ErrorOr<RecipeDto>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Recipe recipe =
				await context.Recipes.SingleOrDefaultAsync(r => r.Id == request.Recipe.Id, cancellationToken)
				?? throw new Exception($"recipe by id:{request.Recipe.Id} not found.");

			recipe.Name = request.Recipe.Name;

			// Reconcile the recipe's items with the set provided in the request:
			// update the ones still present, remove the missing, add the new.
			recipe.Ingredients.Sync(request.Recipe.Ingredients, keyOfExisting: item => item.Id,
									keyOfIncoming: dto => dto.Id, create: dto => dto.ToEntity(),
									update: (dto, item) => dto.ApplyTo(item));

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
