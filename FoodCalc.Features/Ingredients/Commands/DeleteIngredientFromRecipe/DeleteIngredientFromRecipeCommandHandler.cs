using ErrorOr;
using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredients.Commands.DeleteIngredientFromRecipe;
public class DeleteIngredientFromRecipeCommandHandler(FoodHubDbContext context, ILogger<DeleteIngredientFromRecipeCommandHandler> logger) : IRequestHandler<DeleteIngredientFromRecipeCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(DeleteIngredientFromRecipeCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var recipeItem = await context.RecipeItems.SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
			if (recipeItem != null)
			{
				context.RecipeItems.Remove(recipeItem);
				await context.SaveChangesAsync(cancellationToken);
			}

			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Ingredient.DeleteFromRecipeFailed);
			return Error.Failure(description: ErrorMessages.Ingredient.DeleteFromRecipeFailed);
		}
	}
}
