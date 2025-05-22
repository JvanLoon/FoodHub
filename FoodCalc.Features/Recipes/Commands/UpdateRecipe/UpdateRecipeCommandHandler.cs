using ErrorOr;
using MediatR;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;
public class UpdateRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRecipeCommandHandler> logger) : IRequestHandler<UpdateRecipeCommand, ErrorOr<Recipe>>
{
	public async Task<ErrorOr<Recipe>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Recipe recipe = await unitOfWork.RecipeRepository.GetByIdAsync(request.Recipe.Id, cancellationToken) ??
							throw new Exception($"recipe by id:{request.Recipe.Id} not found.");

			if (!string.IsNullOrWhiteSpace(request.Recipe.Name))
			{
				recipe.Name = request.Recipe.Name;
			}

			recipe.RecipeIngredient.Clear();

			foreach (RecipeIngredient ingredient in request.Recipe.RecipeIngredient)
			{
				recipe.RecipeIngredient.Add(ingredient);
			}

			await unitOfWork.RecipeRepository.UpdateAsync(recipe, cancellationToken);
			await unitOfWork.SaveChangesAsync(cancellationToken);

			return request.Recipe;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to update recipe");
			return Error.Failure("Failed to update recipe", ex.Message);
		}
	}
}
