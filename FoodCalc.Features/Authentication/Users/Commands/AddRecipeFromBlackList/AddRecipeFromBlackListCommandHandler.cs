using ErrorOr;

using FoodCalc.Features.Recipes.Commands.DeleteRecipe;

using FoodHub.Persistence.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCalc.Features.Authentication.Users.Commands.AddRecipeFromBlackList;
public class AddRecipeFromBlackListCommandHandler(IUnitOfWork unitOfWork, ILogger<AddRecipeFromBlackListCommandHandler> logger) : IRequestHandler<AddRecipeFromBlackListCommand, ErrorOr<bool>>
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
