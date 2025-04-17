using ErrorOr;

using FoodCalc.Features.Recepts.Queries.GetAllRecepts;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories.Interface;

using MediatR;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recepts.Queries.GetById;
public class GetRecipeByIdQueryHandler(IRecipeRepository receptRepository, ILogger<GetRecipeByIdQueryHandler> logger) : IRequestHandler<GetRecipeByIdQuery, ErrorOr<Recipe?>>
{
    public async Task<ErrorOr<Recipe?>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var recept = await receptRepository.GetByIdAsync(request.Id, cancellationToken);

            if (recept is null)
            {
                return Error.Failure("Recipe not found");
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
