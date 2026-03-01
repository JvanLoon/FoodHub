using ErrorOr;
using MediatR;
using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Ingredients.Commands.AddIngredient
{
	public class AddIngredientCommandHandler(UnitOfWork unitOfWork, IMapper mapper, ILogger<AddIngredientCommandHandler> logger)
		: IRequestHandler<AddIngredientCommand, ErrorOr<IngredientDto>>
	{
		public async Task<ErrorOr<IngredientDto>> Handle(AddIngredientCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var ingredient = mapper.Map<Ingredient>(request.Ingredient);
				await unitOfWork.IngredientRepository.AddAsync(ingredient, cancellationToken);

				return mapper.Map<IngredientDto>(ingredient);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to add ingredient");
				return Error.Failure("Failed to add ingredient");
			}
		}
	}
}
