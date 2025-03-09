using ErrorOr;
using MediatR;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.ApiService.Features.Recepts.Commands.AddRecept;
public class AddReceptCommandHandler(IUnitOfWork unitOfWork, ILogger<AddReceptCommandHandler> logger) : IRequestHandler<AddReceptCommand, ErrorOr<Recept>>
{
    public async Task<ErrorOr<Recept>> Handle(AddReceptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Recept? recept = await unitOfWork.ReceptRepository.AddAsync(request.recept, cancellationToken);

            if (recept == null) {
                return Error.Failure("Failed to add recept");
            }

            return recept;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add recept");
            return Error.Failure("Failed to add recept");
        }
    }
}
