using FoodCalcHub.ApiService.Persistence;
using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Commands.DeleteRecept;
public class DeleteReceptCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteReceptCommand, Guid>
{
    public async Task<Guid> Handle(DeleteReceptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.ReceptRepository.DeleteAsync(request.Id, cancellationToken);

            return request.Id;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to delete recept", ex);
        }
    }
}