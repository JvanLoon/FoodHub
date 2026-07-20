using ErrorOr;

using MediatR;

using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Authentication.Users.Commands.AddRecipeFromBlackList;
public class AddRecipeFromBlackListCommandHandler(FoodHubDbContext context, ILogger<AddRecipeFromBlackListCommandHandler> logger) : IRequestHandler<AddRecipeFromBlackListCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(AddRecipeFromBlackListCommand request, CancellationToken cancellationToken)
	{
		try
		{
			//await unitOfWork.RecipeRepository.DeleteAsync(request.Id, cancellationToken);

			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to ");
			return Error.Failure("Failed to ");
		}
	}
}
