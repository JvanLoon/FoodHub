using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredients.Commands.DeleteIngredient;

public class DeleteIngredientCommandHandler(FoodHubDbContext context, ILogger<DeleteIngredientCommandHandler> logger)
	: IRequestHandler<DeleteIngredientCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var ingredient = await context.Ingredients.SingleOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
			if (ingredient != null)
			{
				context.Ingredients.Remove(ingredient);
				await context.SaveChangesAsync(cancellationToken);
			}

			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.DeleteFailed("ingredient"));
			return Error.Failure(description: ErrorMessages.Common.DeleteFailed("ingredient"));
		}
	}
}
