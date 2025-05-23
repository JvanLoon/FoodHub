using ErrorOr;

using FoodCalc.Features.Ingredients.Commands.DeleteIngredient;

using FoodHub.Persistence.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredients.Commands.DeleteIngredient;
public class DeleteIngredientCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteIngredientCommandHandler> logger) : IRequestHandler<DeleteIngredientCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await unitOfWork.IngredientRepository.DeleteAsync(request.Id, cancellationToken);

			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to delete ingredient");
			return Error.Failure("Failed to delete ingredient");
		}
	}
}
