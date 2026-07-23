using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredients.Commands.AddIngredient
{
	public class AddIngredientCommandHandler(FoodHubDbContext context, ILogger<AddIngredientCommandHandler> logger)
		: IRequestHandler<AddIngredientCommand, ErrorOr<IngredientDto>>
	{
		public async Task<ErrorOr<IngredientDto>> Handle(AddIngredientCommand request,
														 CancellationToken cancellationToken
		)
		{
			try
			{
				Ingredient ingredient = request.Ingredient.ToEntity();
				context.Ingredients.Add(ingredient);
				await context.SaveChangesAsync(cancellationToken);

				return ingredient.ToDto();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ErrorMessages.Common.AddFailed("ingredient"));
				return Error.Failure(description: ErrorMessages.Common.AddFailed("ingredient"));
			}
		}
	}
}