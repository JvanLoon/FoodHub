using ErrorOr;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddRecipe;
public class AddRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<AddRecipeCommandHandler> logger) : MediatR.IRequestHandler<AddRecipeCommand, ErrorOr<Recipe>>
{
    public async Task<ErrorOr<Recipe>> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Recipe? recipe = await unitOfWork.RecipeRepository.AddAsync(request.recipe, cancellationToken);

            if (recipe == null) {
                return Error.Failure("Failed to add recipe");
            }

            return recipe;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add recipe");
            return Error.Failure("Failed to add recipe");
        }
    }
}
