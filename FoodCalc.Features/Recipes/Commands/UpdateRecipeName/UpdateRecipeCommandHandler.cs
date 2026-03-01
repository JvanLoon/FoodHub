using ErrorOr;
using MediatR;
using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.UpdateRecipeName;
public class UpdateRecipeNameCommandHandler(UnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateRecipeNameCommandHandler> logger) : IRequestHandler<UpdateRecipeNameCommand, ErrorOr<RecipeDto>>
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

			return mapper.Map<RecipeDto>(recipe);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to update recipe");
			return Error.Failure("Failed to update recipe", ex.Message);
		}
	}
}
