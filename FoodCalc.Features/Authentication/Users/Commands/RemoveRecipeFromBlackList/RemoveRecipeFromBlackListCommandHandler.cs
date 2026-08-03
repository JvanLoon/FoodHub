using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Authentication.Users.Commands.RemoveRecipeFromBlackList;

public class RemoveRecipeFromBlackListCommandHandler(
    FoodHubDbContext context,
    ILogger<RemoveRecipeFromBlackListCommandHandler> logger)
    : IRequestHandler<RemoveRecipeFromBlackListCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(RemoveRecipeFromBlackListCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            //await unitOfWork.RecipeRepository.DeleteAsync(request.Id, cancellationToken);

            return await context.RecipeBlackLists.AnyAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to remove recipe from the blacklist");
            return Error.Failure(description: ErrorMessages.User.BlackListRemoveFailed);
        }
    }
}