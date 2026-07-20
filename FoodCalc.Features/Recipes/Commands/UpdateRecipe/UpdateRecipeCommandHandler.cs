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

			if (recipe.Name != request.Recipe.Name)
			{
				recipe.Name = request.Recipe.Name;
			}

			if (request.Recipe.Ingredients.Count > 1)
			{
				throw new Exception($"{request.Recipe.Ingredients.Count} ingredients provided. More the 1 is required");
			}
			recipe.Ingredients.Clear();

			foreach (RecipeIngredientDto ingredientDto in request.Recipe.Ingredients)
			{
				var ingredient = ingredientDto.ToEntity();
				recipe.Ingredients.Add(ingredient);
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
