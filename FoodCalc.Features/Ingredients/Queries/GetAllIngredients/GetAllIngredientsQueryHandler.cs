using ErrorOr;
using FoodCalc.Features;
using FoodCalc.Features.Mapping;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;

public class GetAllIngredientsQueryHandler(FoodHubDbContext context, ILogger<GetAllIngredientsQueryHandler> logger)
    : IRequestHandler<GetAllIngredientsQuery, ErrorOr<PagedResultDto<IngredientDto>>>
{
    public async Task<ErrorOr<PagedResultDto<IngredientDto>>> Handle(GetAllIngredientsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = context.Ingredients.VisibleTo(request.RequestingUserId);

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(i => i.Name.Contains(request.Search));

            return await query.ToPagedResultAsync(request, items => items.ToDtoList(), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.GetAllFailed(ErrorMessages.Entities.Ingredients));
            return Error.Failure(description: ErrorMessages.Common.GetAllFailed(ErrorMessages.Entities.Ingredients));
        }
    }
}