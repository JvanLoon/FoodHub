using ErrorOr;
using MediatR;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredients.Commands.AddIngredient
{
	public class AddIngredientCommandHandler(UnitOfWork unitOfWork, ILogger<AddIngredientCommandHandler> logger)
		: IRequestHandler<AddIngredientCommand, ErrorOr<IngredientDto>>
	{
		public async Task<ErrorOr<IngredientDto>> Handle(AddIngredientCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var ingredient = request.Ingredient.ToEntity();
				await unitOfWork.IngredientRepository.AddAsync(ingredient, cancellationToken);

				return ingredient.ToDto();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ErrorMessages.AddIngredientFailed);
				return Error.Failure(ErrorMessages.AddIngredientFailed);
			}
		}
	}
}
