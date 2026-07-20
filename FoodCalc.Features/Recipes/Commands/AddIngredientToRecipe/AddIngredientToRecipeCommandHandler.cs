using ErrorOr;
using MediatR;
using FoodCalc.Features.Mapping;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;
public class AddIngredientToRecipeCommandHandler(FoodHubDbContext context, ILogger<AddIngredientToRecipeCommandHandler> logger) : IRequestHandler<AddIngredientToRecipeCommand, ErrorOr<RecipeIngredientDto>>
{
    public async Task<ErrorOr<RecipeIngredientDto>> Handle(AddIngredientToRecipeCommand request, CancellationToken cancellationToken)
    {
		try
		{
			var dto = request.RecipeIngredient;

			var existing = await context.RecipeIngredients
				.FirstOrDefaultAsync(ri => ri.Id == dto.Id && ri.RecipeId == dto.RecipeId, cancellationToken);

			if (existing != null)
			{
				existing.Name = dto.Name;
				existing.Amount = dto.Amount;
				existing.IngredientAmount = (IngredientAmountType)dto.IngredientAmount;
				existing.ShouldBeAddedToShoppingCart = dto.ShouldBeAddedToShoppingCart;

				await context.SaveChangesAsync(cancellationToken);
				return existing.ToDto();
			}

			RecipeIngredient mappedRecipeIngredient = dto.ToEntity();
			context.RecipeIngredients.Add(mappedRecipeIngredient);
			await context.SaveChangesAsync(cancellationToken);
			return mappedRecipeIngredient.ToDto();
		}
	    catch (Exception ex)
	    {
		    logger.LogError(ex, ErrorMessages.Recipe.AddIngredientFailed);
		    return Error.Failure(ErrorMessages.Recipe.UpdateForIngredientFailed);
	    }
	}
}
