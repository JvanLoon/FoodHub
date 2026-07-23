using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;

public class AddIngredientToRecipeCommandHandler(
	FoodHubDbContext context,
	ILogger<AddIngredientToRecipeCommandHandler> logger)
	: IRequestHandler<AddIngredientToRecipeCommand, ErrorOr<RecipeItemDto>>
{
	public async Task<ErrorOr<RecipeItemDto>> Handle(AddIngredientToRecipeCommand request,
													 CancellationToken cancellationToken
	)
	{
		try
		{
			var dto = request.RecipeItem;

			var existing = await context.RecipeItems.FirstOrDefaultAsync(
				ri => ri.Id == dto.Id && ri.RecipeId == dto.RecipeId, cancellationToken);

			if (existing != null)
			{
				existing.Name = dto.Name;
				existing.Amount = dto.Amount;
				existing.IngredientAmount = (IngredientAmountType) dto.IngredientAmount;
				existing.ShouldBeAddedToShoppingCart = dto.ShouldBeAddedToShoppingCart;

				await context.SaveChangesAsync(cancellationToken);
				return existing.ToDto();
			}

			RecipeItem mappedRecipeItem = dto.ToEntity();
			context.RecipeItems.Add(mappedRecipeItem);
			await context.SaveChangesAsync(cancellationToken);
			return mappedRecipeItem.ToDto();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Recipe.AddIngredientFailed);
			return Error.Failure(description: ErrorMessages.Recipe.UpdateForIngredientFailed);
		}
	}
}