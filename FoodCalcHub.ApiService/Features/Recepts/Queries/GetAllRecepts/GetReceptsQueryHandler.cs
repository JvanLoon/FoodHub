using FoodCalcHub.ApiService.Repositories;
using FoodCalcHub.ApiService.Repositories.Interface;

namespace FoodCalcHub.ApiService.Features.Recepts.Queries.GetAllRecepts;
public class GetReceptsQueryHandler(IReceptRepository receptRepository)
{
    public async Task<IEnumerable<Entities.Recept>> Handle(GetAllReceptsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await receptRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get all recepts", ex);
        }
    }
}
