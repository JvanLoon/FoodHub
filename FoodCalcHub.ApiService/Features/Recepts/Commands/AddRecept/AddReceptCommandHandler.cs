using FoodCalcHub.ApiService.Entities;
using FoodCalcHub.ApiService.Persistence;
using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Commands.AddRecept;
public class AddReceptCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddReceptCommand, Recept>
{
    public Task<Recept> Handle(AddReceptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //todo: add validation
            return unitOfWork.ReceptRepository.AddAsync(request.recept, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to add recept", ex);
        }
    }
}
