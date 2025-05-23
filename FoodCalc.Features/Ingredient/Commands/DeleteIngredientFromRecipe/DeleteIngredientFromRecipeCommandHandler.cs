using ErrorOr;
using MediatR;

using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.DeleteIngredientFromRecipe;
public class DeleteIngredientFromRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteIngredientFromRecipeCommandHandler> logger) : IRequestHandler<DeleteIngredientFromRecipeCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(DeleteIngredientFromRecipeCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await unitOfWork.RecipeRepository.DeleteRecipeIngredientAsync(request.Id, cancellationToken);

			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to delete ingredient from recipe");
			return Error.Failure("Failed to delete ingredient from recipe");
		}
	}
}
