using ErrorOr;
using MediatR;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recepts.Commands.UpdateRecept;
public class UpdateReceptCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateReceptCommandHandler> logger) : IRequestHandler<UpdateReceptCommand, ErrorOr<Recept>>
{
	public async Task<ErrorOr<Recept>> Handle(UpdateReceptCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Recept recept = await unitOfWork.ReceptRepository.GetByIdAsync(request.Recept.Id, cancellationToken) ??
							throw new Exception($"recept by id:{request.Recept.Id} not found.");

			if (!string.IsNullOrWhiteSpace(request.Recept.Name))
			{
				recept.Name = request.Recept.Name;
			}

			recept.ReceptIngredient.Clear();

			foreach (ReceptIngredient ingredient in request.Recept.ReceptIngredient)
			{
				recept.ReceptIngredient.Add(ingredient);
			}

			await unitOfWork.ReceptRepository.UpdateAsync(recept, cancellationToken);
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
