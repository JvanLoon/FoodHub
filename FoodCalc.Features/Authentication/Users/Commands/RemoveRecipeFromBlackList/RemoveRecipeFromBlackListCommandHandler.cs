using ErrorOr;

using MediatR;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Authentication.Users.Commands.RemoveRecipeFromBlackList;

public class RemoveRecipeFromBlackListCommandHandler(FoodHubDbContext context, ILogger<RemoveRecipeFromBlackListCommandHandler> logger) : IRequestHandler<RemoveRecipeFromBlackListCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(RemoveRecipeFromBlackListCommand request, CancellationToken cancellationToken)
	{
		try
		{
			//await unitOfWork.RecipeRepository.DeleteAsync(request.Id, cancellationToken);

			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to ");
			return Error.Failure(description: "Failed to ");
		}
	}
}
