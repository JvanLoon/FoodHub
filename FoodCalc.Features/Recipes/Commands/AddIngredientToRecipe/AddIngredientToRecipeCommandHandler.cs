using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recepts.Commands.AddIngredientToRecept;
public class AddIngredientToRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<AddIngredientToRecipeCommandHandler> logger) : IRequestHandler<AddIngredientToRecipeCommand, ErrorOr<RecipeIngredient>>
{
    public async Task<ErrorOr<RecipeIngredient>> Handle(AddIngredientToRecipeCommand request, CancellationToken cancellationToken)
    {
		try
	    {
		    await unitOfWork.RecipeRepository.AddReceptIngredientAsync(request.ReceptIngredient, cancellationToken);

			return request.ReceptIngredient;
		}
	    catch (Exception ex)
	    {
		    logger.LogError(ex, "Failed to add ingredient to Recipe");
		    return Error.Failure("Failed to update Recipe");
	    }
	}
}
