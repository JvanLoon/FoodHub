using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recepts.Commands.AddIngredientToRecept;
public class AddIngredientToReceptCommandHandler(IUnitOfWork unitOfWork, ILogger<AddIngredientToReceptCommandHandler> logger) : IRequestHandler<AddIngredientToReceptCommand, ErrorOr<Recept>>
{
    public async Task<ErrorOr<Recept>> Handle(AddIngredientToReceptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Recept? recept = await unitOfWork.ReceptRepository.GetByIdAsync(request.ReceptId, cancellationToken);

            if (recept == null)
            {
                throw new Exception($"Recept with id: {request.ReceptId} not found");
            }

            recept.Ingredients.Add(request.Ingredient);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return recept;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add ingredient to recept");
            return Error.Failure("Failed to update recept");
        }
    }
}
