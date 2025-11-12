
using ErrorOr;

using FoodCalc.Features.ImportExport.Export.Commands.ExportJSON;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;
public class ImportAllCommandHandler(IUnitOfWork unitOfWork, ILogger<ImportAllCommandHandler> logger) : IRequestHandler<ImportAllCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(ImportAllCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var data = request.Data;
			if (data == null)
				return Error.Failure("No data provided for import.");

			// Import Ingredients
			foreach (var ingredientDto in data.Ingredients)
			{
				// Check if ingredient exists (by Id or Name, adjust as needed)
				var existing = await unitOfWork.IngredientRepository.GetByIdAsync(ingredientDto.Id, cancellationToken);
				if (existing == null)
				{
					var ingredient = new Ingredient
					{
						Id = ingredientDto.Id,
						Name = ingredientDto.Name,
						// Map other properties as needed
					};
					await unitOfWork.IngredientRepository.AddAsync(ingredient, cancellationToken);
				}
				// else: update or skip
			}

			// Import Recipes
			foreach (var recipeDto in data.Recipes)
			{
				var existing = await unitOfWork.RecipeRepository.GetByIdAsync(recipeDto.Id, cancellationToken);
				if (existing == null)
				{
					var recipe = new Recipe
					{
						Id = recipeDto.Id,
						Name = recipeDto.Name,
						// Map other properties as needed
					};
					await unitOfWork.RecipeRepository.AddAsync(recipe, cancellationToken);
				}
				// else: update or skip
			}

			// Import RecipeIngredients
			foreach (var riDto in data.RecipeIngredients)
			{
				// You may want to check for existence or just add
				var recipeIngredient = new RecipeIngredient
				{
					Id = riDto.Id,
					RecipeId = riDto.RecipeId,
					IngredientId = riDto.IngredientId,
					// Map other properties as needed
				};
				await unitOfWork.RecipeRepository.AddRecipeIngredientAsync(recipeIngredient, cancellationToken);
			}

			// Import Users with Roles (optional)
			if (data.Users != null)
			{
				foreach (var userDto in data.Users)
				{
					var existing = await unitOfWork.UserRepository.GetByEmailAsync(userDto.Email, cancellationToken);
					if (existing == null)
					{
						var user = new IdentityUser
						{
							Id = userDto.Id,
							Email = userDto.Email,
							UserName = userDto.Email,
							EmailConfirmed = true
						};
						// You may need to set a default password or handle this elsewhere
						await unitOfWork.UserRepository.UpdateAsync(user, cancellationToken);
						// Add roles
						foreach (var role in userDto.Roles)
						{
							await unitOfWork.UserRepository.AddRoleToUser(user, role, cancellationToken);
						}
					}
					// else: update or skip
				}
			}

			await unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to import all data");
			return Error.Failure("Failed to import all data");
		}
	}
}
