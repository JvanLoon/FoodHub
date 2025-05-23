using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;
public class AddIngredientToRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<AddIngredientToRecipeCommandHandler> logger) : IRequestHandler<AddIngredientToRecipeCommand, ErrorOr<RecipeIngredient>>
{
    public async Task<ErrorOr<RecipeIngredient>> Handle(AddIngredientToRecipeCommand request, CancellationToken cancellationToken)
    {
		try
		{
			var recipeIngredient = await unitOfWork.RecipeRepository.GetIngredientRecipeByRecipeId(request.RecipeIngredient.RecipeId, request.RecipeIngredient.Id, cancellationToken);

			if (recipeIngredient != null)
			{
				recipeIngredient.Amount = request.RecipeIngredient.Amount;
				recipeIngredient.IngredientAmount = request.RecipeIngredient.IngredientAmount;

				await unitOfWork.IngredientRepository.UpdateAsync(recipeIngredient.Ingredient, cancellationToken);
			}
			else
			{
				await unitOfWork.RecipeRepository.AddRecipeIngredientAsync(request.RecipeIngredient, cancellationToken);
			}

			return request.RecipeIngredient;
		}
	    catch (Exception ex)
	    {
		    logger.LogError(ex, "Failed to add ingredient to Recipe");
		    return Error.Failure("Failed to update Recipe");
	    }
	}
}
