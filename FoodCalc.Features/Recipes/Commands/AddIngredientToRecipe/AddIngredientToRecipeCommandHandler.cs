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
		    await unitOfWork.RecipeRepository.AddRecipeIngredientAsync(request.RecipeIngredient, cancellationToken);

			return request.RecipeIngredient;
		}
	    catch (Exception ex)
	    {
		    logger.LogError(ex, "Failed to add ingredient to Recipe");
		    return Error.Failure("Failed to update Recipe");
	    }
	}
}
