using FoodCalcHub.ApiService.Entities;
using FoodCalcHub.ApiService.Persistence;
using MediatR;


namespace FoodCalcHub.ApiService.Features.Recepts.Commands.UpdateRecept;
public class UpdateReceptCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateReceptCommand, Recept>
{
    public async Task<Recept> Handle(UpdateReceptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Recept recept = await unitOfWork.ReceptRepository.GetByIdAsync(request.Recept.Id, cancellationToken);

            if (recept == null)
            {
                throw new Exception($"Recept with id: {request.Recept.Id} not found");
            }

            recept.Name = request.Recept.Name;
            recept.Ingredients = request.Recept.Ingredients;

            await unitOfWork.SaveChangesAsync();

            return recept;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to update recept", ex);
        }
    }
}
