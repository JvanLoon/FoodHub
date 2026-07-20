using ErrorOr;
using MediatR;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;
public class UpdateRecipeCommandHandler(FoodHubDbContext context, ILogger<UpdateRecipeCommandHandler> logger) : IRequestHandler<UpdateRecipeCommand, ErrorOr<RecipeDto>>
{
	public async Task<ErrorOr<RecipeDto>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Recipe recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == request.Recipe.Id, cancellationToken) ??
							throw new Exception($"recipe by id:{request.Recipe.Id} not found.");

			recipe.Name = request.Recipe.Name;

			// Replace the recipe's items with the set provided in the request.
			recipe.Ingredients.Clear();
			foreach (RecipeItemDto itemDto in request.Recipe.Ingredients)
			{
				recipe.Ingredients.Add(itemDto.ToEntity());
			}

			await context.SaveChangesAsync(cancellationToken);

			return recipe.ToDto();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.UpdateFailed("recipe"));
			return Error.Failure(ErrorMessages.Common.UpdateFailed("recipe"), ex.Message);
		}
	}
}
