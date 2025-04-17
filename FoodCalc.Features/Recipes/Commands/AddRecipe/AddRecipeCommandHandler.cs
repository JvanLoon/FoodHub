using ErrorOr;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recepts.Commands.AddRecept;
public class AddRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<AddRecipeCommandHandler> logger) : MediatR.IRequestHandler<AddRecipeCommand, ErrorOr<Recipe>>
{
    public async Task<ErrorOr<Recipe>> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Recipe? recept = await unitOfWork.RecipeRepository.AddAsync(request.recept, cancellationToken);

            if (recept == null) {
                return Error.Failure("Failed to add recept");
            }

            return recept;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add recept");
            return Error.Failure("Failed to add recept");
        }
    }
}
