using ErrorOr;

using FoodCalcHub.ApiService.Entities;
using FoodCalcHub.ApiService.Persistence;
using FoodCalcHub.ApiService.Repositories;
using FoodCalcHub.ApiService.Repositories.Interface;

using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Queries.GetAllRecepts;
public class GetReceptsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetReceptsQueryHandler> logger) : IRequestHandler<GetAllReceptsQuery, ErrorOr<List<Recept>>>
{
    public async Task<ErrorOr<List<Recept>>> Handle(GetAllReceptsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ReceptRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get all recepts");
            return Error.Failure("Failed to get all recepts");
        }
    }
}
