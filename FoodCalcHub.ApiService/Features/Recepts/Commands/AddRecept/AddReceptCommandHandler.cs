using ErrorOr;

using FoodCalcHub.ApiService.Entities;
using FoodCalcHub.ApiService.Persistence;
using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Commands.AddRecept;
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
