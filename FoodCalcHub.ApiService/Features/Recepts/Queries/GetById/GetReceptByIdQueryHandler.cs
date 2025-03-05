using FoodCalcHub.ApiService.Entities;
using FoodCalcHub.ApiService.Repositories.Interface;

namespace FoodCalcHub.ApiService.Features.Recepts.Queries.GetById;
public class GetReceptByIdQueryHandler(IReceptRepository receptRepository)
{
    public async Task<Recept?> Handle(GetReceptByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await receptRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get recept by id: {request.Id}", ex);
        }
    }
}