using ErrorOr;
using MediatR;
using FoodCalc.Features.Mapping;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;
public class AddIngredientToRecipeCommandHandler(UnitOfWork unitOfWork, ILogger<AddIngredientToRecipeCommandHandler> logger) : IRequestHandler<AddIngredientToRecipeCommand, ErrorOr<RecipeIngredientDto>>
{
    public async Task<ErrorOr<RecipeIngredientDto>> Handle(AddIngredientToRecipeCommand request, CancellationToken cancellationToken)
    {
		try
		{
			var recipeIngredient = await unitOfWork.RecipeRepository.GetIngredientRecipeByRecipeId(request.RecipeIngredient.RecipeId, request.RecipeIngredient.Id, cancellationToken);

			if (recipeIngredient != null)
			{
				recipeIngredient.Amount = request.RecipeIngredient.Amount;
				recipeIngredient.IngredientAmount = (IngredientAmountType)request.RecipeIngredient.IngredientAmount;

				await unitOfWork.IngredientRepository.UpdateAsync(recipeIngredient.Ingredient, cancellationToken);
				return recipeIngredient.ToDto();
			}
			else
			{
				RecipeIngredient mappedRecipeIngredient = request.RecipeIngredient.ToEntity();
				await unitOfWork.RecipeRepository.AddRecipeIngredientAsync(mappedRecipeIngredient, cancellationToken);
				return mappedRecipeIngredient.ToDto();
			}
		}
	    catch (Exception ex)
	    {
		    logger.LogError(ex, "Failed to add ingredient to Recipe");
		    return Error.Failure("Failed to update Recipe");
	    }
	}
}
