using ErrorOr;
using MediatR;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Feature.Ingredient.Commands.AddIngredient
{
	internal class AddIngredientCommandHandler(IUnitOfWork unitOfWork, ILogger<AddIngredientCommandHandler> logger)
		: IRequestHandler<AddIngredientCommand, ErrorOr<Success>>
	{
		public async Task<ErrorOr<Success>> Handle(AddIngredientCommand request, CancellationToken cancellationToken)
		{
			try
			{
				await unitOfWork.IngredientRepository.AddAsync(request.Ingredient, cancellationToken);

				return Result.Success;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to add ingredient to recept");
				return Error.Failure("Failed to update recept");
			}
		}
	}
}
