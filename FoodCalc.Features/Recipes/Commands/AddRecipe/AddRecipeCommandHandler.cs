using ErrorOr;
using FoodCalc.Features.Mapping;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddRecipe;
public class GetUserByEmailHandler(UnitOfWork unitOfWork, ILogger<GetUserByEmailHandler> logger) : MediatR.IRequestHandler<AddRecipeCommand, ErrorOr<RecipeDto>>
{
    public async Task<ErrorOr<RecipeDto>> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = request.recipe.ToEntity();
            Recipe? addedRecipe = await unitOfWork.RecipeRepository.AddAsync(recipe, cancellationToken);

            if (addedRecipe == null) {
                return Error.Failure("Failed to add recipe");
            }

            return addedRecipe.ToDto();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add recipe");
            return Error.Failure("Failed to add recipe");
        }
    }
}
