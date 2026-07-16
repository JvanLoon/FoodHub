using ErrorOr;
using MediatR;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipeName;
public class UpdateRecipeNameCommandHandler(UnitOfWork unitOfWork, ILogger<UpdateRecipeNameCommandHandler> logger) : IRequestHandler<UpdateRecipeNameCommand, ErrorOr<RecipeDto>>
{
	public async Task<ErrorOr<RecipeDto>> Handle(UpdateRecipeNameCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Recipe recipe = await unitOfWork.RecipeRepository.GetByIdAsync(request.RecipeId, cancellationToken) ??
							throw new Exception($"recipe by id:{request.RecipeId} not found.");

			if (!string.IsNullOrWhiteSpace(request.newRecipeName))
			{
				recipe.Name = request.newRecipeName;
			}

			await unitOfWork.RecipeRepository.UpdateNameAsync(recipe, cancellationToken);

			return recipe.ToDto();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.UpdateFailed("recipe"));
			return Error.Failure(ErrorMessages.Common.UpdateFailed("recipe"), ex.Message);
		}
	}
}
