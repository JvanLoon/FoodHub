using ErrorOr;
using MediatR;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recepts.Commands.UpdateRecept;
public class UpdateReceptCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateReceptCommandHandler> logger) : IRequestHandler<UpdateReceptCommand, ErrorOr<Recipe>>
{
	public async Task<ErrorOr<Recipe>> Handle(UpdateReceptCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Recipe recept = await unitOfWork.RecipeRepository.GetByIdAsync(request.Recept.Id, cancellationToken) ??
							throw new Exception($"recept by id:{request.Recept.Id} not found.");

			if (!string.IsNullOrWhiteSpace(request.Recept.Name))
			{
				recept.Name = request.Recept.Name;
			}

			recept.RecipeIngredient.Clear();

			foreach (RecipeIngredient ingredient in request.Recept.RecipeIngredient)
			{
				recept.RecipeIngredient.Add(ingredient);
			}

			await unitOfWork.RecipeRepository.UpdateAsync(recept, cancellationToken);
			await unitOfWork.SaveChangesAsync(cancellationToken);

			return request.Recept;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to update recept");
			return Error.Failure("Failed to update recept", ex.Message);
		}
	}
}
