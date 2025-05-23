using ErrorOr;
using MediatR;

using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.DeleteRecipe;
public class DeleteRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteRecipeCommandHandler> logger) : IRequestHandler<DeleteRecipeCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await unitOfWork.RecipeRepository.DeleteAsync(request.Id, cancellationToken);

			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to delete recipe");
			return Error.Failure("Failed to delete recipe");
		}
	}
}
