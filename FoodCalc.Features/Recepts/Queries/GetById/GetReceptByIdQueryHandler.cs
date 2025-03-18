using ErrorOr;

using FoodCalc.Features.Recepts.Queries.GetAllRecepts;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories.Interface;

using MediatR;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recepts.Queries.GetById;
public class GetReceptByIdQueryHandler(IReceptRepository receptRepository, ILogger<GetReceptByIdQueryHandler> logger) : IRequestHandler<GetReceptByIdQuery, ErrorOr<Recept?>>
{
    public async Task<ErrorOr<Recept?>> Handle(GetReceptByIdQuery request, CancellationToken cancellationToken)
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
