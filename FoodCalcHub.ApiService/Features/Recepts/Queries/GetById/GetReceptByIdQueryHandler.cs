using ErrorOr;

using FoodCalcHub.ApiService.Entities;
using FoodCalcHub.ApiService.Repositories.Interface;

namespace FoodCalcHub.ApiService.Features.Recepts.Queries.GetById;
public class GetReceptByIdQueryHandler(IReceptRepository receptRepository, ILogger<GetReceptByIdQueryHandler> logger)
{
    public async Task<ErrorOr<Recept>> Handle(GetReceptByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var recept = await receptRepository.GetByIdAsync(request.Id, cancellationToken);

            if (recept is null)
            {
                return Error.Failure("Recept not found");
            }
            return recept;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to get recept by id: {request.Id}");
            return Error.Failure($"Failed to get recept by id: {request.Id}");
        }
    }
}
