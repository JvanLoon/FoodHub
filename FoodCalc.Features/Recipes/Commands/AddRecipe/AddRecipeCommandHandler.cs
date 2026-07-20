using ErrorOr;
using FoodCalc.Features.Mapping;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddRecipe;
public class AddRecipeCommandHandler(FoodHubDbContext context, ILogger<AddRecipeCommandHandler> logger) : MediatR.IRequestHandler<AddRecipeCommand, ErrorOr<RecipeDto>>
{
    public async Task<ErrorOr<RecipeDto>> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Recipe recipe = request.recipe.ToEntity();
            context.Recipes.Add(recipe);
            await context.SaveChangesAsync(cancellationToken);

            return recipe.ToDto();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.AddFailed("recipe"));
            return Error.Failure(ErrorMessages.Common.AddFailed("recipe"));
        }
    }
}
