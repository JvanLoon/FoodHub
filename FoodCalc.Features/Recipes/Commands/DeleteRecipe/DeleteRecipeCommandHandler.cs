using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.DeleteRecipe;

public class DeleteRecipeCommandHandler(FoodHubDbContext context, ILogger<DeleteRecipeCommandHandler> logger)
	: IRequestHandler<DeleteRecipeCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
			if (recipe != null)
			{
				context.Recipes.Remove(recipe);
				await context.SaveChangesAsync(cancellationToken);
			}

			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.DeleteFailed("recipe"));
			return Error.Failure(description: ErrorMessages.Common.DeleteFailed("recipe"));
		}
	}
}
