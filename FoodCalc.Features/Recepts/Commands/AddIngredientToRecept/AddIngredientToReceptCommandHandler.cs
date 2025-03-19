using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recepts.Commands.AddIngredientToRecept;
public class AddIngredientToReceptCommandHandler(IUnitOfWork unitOfWork, ILogger<AddIngredientToReceptCommandHandler> logger) : IRequestHandler<AddIngredientToReceptCommand, ErrorOr<ReceptIngredient>>
{
    public async Task<ErrorOr<ReceptIngredient>> Handle(AddIngredientToReceptCommand request, CancellationToken cancellationToken)
    {
		try
	    {
		    await unitOfWork.ReceptRepository.AddReceptIngredientAsync(request.ReceptIngredient, cancellationToken);

			return request.ReceptIngredient;
		}
	    catch (Exception ex)
	    {
		    logger.LogError(ex, "Failed to add ingredient to recept");
		    return Error.Failure("Failed to update recept");
	    }
	}
}
