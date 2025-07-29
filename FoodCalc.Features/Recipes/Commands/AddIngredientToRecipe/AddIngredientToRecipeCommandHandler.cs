using ErrorOr;
using MediatR;
using AutoMapper;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;
public class AddIngredientToRecipeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddIngredientToRecipeCommandHandler> logger) : IRequestHandler<AddIngredientToRecipeCommand, ErrorOr<RecipeIngredientDto>>
{
    public async Task<ErrorOr<RecipeIngredientDto>> Handle(AddIngredientToRecipeCommand request, CancellationToken cancellationToken)
    {
		try
		{
			var recipeIngredient = await unitOfWork.RecipeRepository.GetIngredientRecipeByRecipeId(request.RecipeIngredient.RecipeId, request.RecipeIngredient.Id, cancellationToken);

			if (recipeIngredient != null)
			{
				recipeIngredient.Amount = request.RecipeIngredient.Amount;
				recipeIngredient.IngredientAmount = mapper.Map<IngredientAmountType>(request.RecipeIngredient.IngredientAmount);

				await unitOfWork.IngredientRepository.UpdateAsync(recipeIngredient.Ingredient, cancellationToken);
				return mapper.Map<RecipeIngredientDto>(recipeIngredient);
			}
			else
			{
				RecipeIngredient mappedRecipeIngredient = mapper.Map<RecipeIngredient>(request.RecipeIngredient);
				await unitOfWork.RecipeRepository.AddRecipeIngredientAsync(mappedRecipeIngredient, cancellationToken);
				return mapper.Map<RecipeIngredientDto>(mappedRecipeIngredient);
			}
		}
	    catch (Exception ex)
	    {
		    logger.LogError(ex, "Failed to add ingredient to Recipe");
		    return Error.Failure("Failed to update Recipe");
	    }
	}
}
