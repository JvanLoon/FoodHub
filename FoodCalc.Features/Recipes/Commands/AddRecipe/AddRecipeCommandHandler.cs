using ErrorOr;
using AutoMapper;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddRecipe;
public class AddRecipeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddRecipeCommandHandler> logger) : MediatR.IRequestHandler<AddRecipeCommand, ErrorOr<RecipeDto>>
{
    public async Task<ErrorOr<RecipeDto>> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = mapper.Map<Recipe>(request.recipe);
            Recipe? addedRecipe = await unitOfWork.RecipeRepository.AddAsync(recipe, cancellationToken);

            if (addedRecipe == null) {
                return Error.Failure("Failed to add recipe");
            }

            return mapper.Map<RecipeDto>(addedRecipe);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add recipe");
            return Error.Failure("Failed to add recipe");
        }
    }
}
