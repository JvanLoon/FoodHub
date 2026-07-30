using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Queries.GetById;

public class GetRecipeByIdQueryHandler(FoodHubDbContext context, ILogger<GetRecipeByIdQueryHandler> logger)
    : IRequestHandler<GetRecipeByIdQuery, ErrorOr<RecipeDto?>>
{
    public async Task<ErrorOr<RecipeDto?>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await context.Recipes.VisibleTo(request.RequestingUserId)
                .SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            // NotFound, not Failure, so this leaves as a 404 — and note it is also what a
            // caller gets for someone else's unapproved recipe: the visibility filter above
            // removes it from the query entirely, so "exists but is not yours" is
            // indistinguishable from "does not exist", which is the intended answer.
            if (recipe is null) { return Error.NotFound(description: ErrorMessages.Common.NotFound(ErrorMessages.Entities.Recipe)); }

            return recipe.ToDto();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to get recipe by id: {request.Id}");
            return Error.Failure(description: ErrorMessages.Recipe.GetByIdFailed(request.Id));
        }
    }
}