using FoodCalcHub.ApiService.Entities;
using FoodCalcHub.ApiService.Features.Recepts.Commands.UpdateRecept;
using FoodCalcHub.ApiService.Persistence;
using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Commands.AddIngredientToRecept;
public class AddIngredientToReceptCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddIngredientToReceptCommand, Recept>
{
    public async Task<Recept> Handle(AddIngredientToReceptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Recept recept = await unitOfWork.ReceptRepository.GetByIdAsync(request.ReceptId, cancellationToken);

            if (recept == null)
            {
                throw new Exception($"Recept with id: {request.ReceptId} not found");
            }

            recept.Ingredients.Add(request.Ingredient);

            await unitOfWork.SaveChangesAsync();

            return recept;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to update recept", ex);
        }
    }
}