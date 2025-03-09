using ErrorOr;
using FoodHub.Persistence.Persistence;
using MediatR;

using Microsoft.Extensions.Logging;

namespace FoodCalc.ApiService.Features.Recepts.Commands.DeleteRecept;
public class DeleteReceptCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteReceptCommandHandler> logger) : IRequestHandler<DeleteReceptCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(DeleteReceptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.ReceptRepository.DeleteAsync(request.Id, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete recept");
            return Error.Failure("Failed to delete recept");
        }
    }
}
