using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Authentication.Users.Commands.AddRecipeFromBlackList;

public class AddRecipeFromBlackListCommandHandler(
    FoodHubDbContext context,
    ILogger<AddRecipeFromBlackListCommandHandler> logger)
    : IRequestHandler<AddRecipeFromBlackListCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(AddRecipeFromBlackListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //await unitOfWork.RecipeRepository.DeleteAsync(request.Id, cancellationToken);

            return await context.RecipeBlackLists.AnyAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add recipe to the blacklist");
            return Error.Failure(description: ErrorMessages.User.BlackListAddFailed);
        }
    }
}