using ErrorOr;
using MediatR;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredient.Commands.AddIngredient
{
	public class AddIngredientCommandHandler(IUnitOfWork unitOfWork, ILogger<AddIngredientCommandHandler> logger)
		: IRequestHandler<AddIngredientCommand, ErrorOr<FoodHub.Persistence.Entities.Ingredient>>
	{
		public async Task<ErrorOr<FoodHub.Persistence.Entities.Ingredient>> Handle(AddIngredientCommand request, CancellationToken cancellationToken)
		{
			try
			{
				await unitOfWork.IngredientRepository.AddAsync(request.Ingredient, cancellationToken);

				return request.Ingredient;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to add ingredient to recept");
				return Error.Failure("Failed to update recept");
			}
		}
	}
}
