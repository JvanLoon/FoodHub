using ErrorOr;
using MediatR;
using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredients.Commands.UpdateIngredient;
public class UpdateIngredientCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateIngredientCommandHandler> logger) : IRequestHandler<UpdateIngredientCommand, ErrorOr<IngredientDto>>
{
	public async Task<ErrorOr<IngredientDto>> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Ingredient ingredient = await unitOfWork.IngredientRepository.GetByIdAsync(request.Ingredient.Id, cancellationToken) ??
							throw new Exception($"ingredient by id:{request.Ingredient.Id} not found.");

			ingredient.Name = request.Ingredient.Name;
			ingredient.ShouldBeAddedToShoppingCart = request.Ingredient.ShouldBeAddedToShoppingCart;

			await unitOfWork.IngredientRepository.UpdateAsync(ingredient, cancellationToken);

			return mapper.Map<IngredientDto>(ingredient);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to update ingredient");
			return Error.Failure("Failed to update ingredient", ex.Message);
		}
	}
}
