using ErrorOr;
using MediatR;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipe;
public class UpdateRecipeNameCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRecipeNameCommandHandler> logger) : IRequestHandler<UpdateRecipeNameCommand, ErrorOr<Recipe>>
{
	public async Task<ErrorOr<Recipe>> Handle(UpdateRecipeNameCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Recipe recipe = await unitOfWork.RecipeRepository.GetByIdAsync(request.RecipeId, cancellationToken) ??
							throw new Exception($"recipe by id:{request.RecipeId} not found.");

			if (!string.IsNullOrWhiteSpace(request.newRecipeName))
			{
				recipe.Name = request.newRecipeName;
			}

			await unitOfWork.RecipeRepository.UpdateNameAsync(recipe, cancellationToken);

			return recipe;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to update recipe");
			return Error.Failure("Failed to update recipe", ex.Message);
		}
	}
}
