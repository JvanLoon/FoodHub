using AutoMapper;

using ErrorOr;

using FoodCalc.Features.Authentication.Users.Commands.RemoveRecipeFromBlackList;
using FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Text.Json;


namespace FoodCalc.Features.ImportExport.Export.Commands.ExportJSON;

public class ExportAllCommandHandler(UnitOfWork unitOfWork, IMapper mapper, ILogger<ExportAllCommandHandler> logger) : IRequestHandler<ExportAllCommand, ErrorOr<string>>
{
	public async Task<ErrorOr<string>> Handle(ExportAllCommand request, CancellationToken cancellationToken)
	{
		try
		{
			// Fetch all data
			var recipes = unitOfWork.RecipeRepository.GetAllAsync();
			var ingredients = unitOfWork.IngredientRepository.GetAllAsync();

			// Assuming RecipeIngredient is a separate entity, fetch all
			var recipeIngredients = new List<RecipeIngredient>();
			foreach (var recipe in recipes)
			{
				if (recipe.RecipeIngredient != null && recipe.RecipeIngredient.Count > 0)
				{
					//remove all ingredients from recipe.RecipeIngredient
					foreach (var recipeIngredient in recipe.RecipeIngredient)
					{
						recipeIngredient.Ingredient = null!;
					}

					recipeIngredients.AddRange(recipe.RecipeIngredient);
				}
			}

			// Optionally fetch users with roles
			List<UserWithRolesDto>? usersWithRoles = null;
			if (request.includeUsers)
			{
				var users = unitOfWork.UserRepository.GetAllAsync();
				usersWithRoles = new List<UserWithRolesDto>();
				foreach (var user in users)
				{
					// You may need to implement a method to get roles for a user
					List<string> roles = await unitOfWork.RoleRepository.GetAllAsync().ToListAsync();
					usersWithRoles.Add(new UserWithRolesDto
					{
						Id = user.Id,
						Email = user.Email ?? "",
						EmailConfirmed = user.EmailConfirmed,
						LockoutEnabled = user.LockoutEnabled,
						Roles = roles // Replace with actual user roles
					});
				}
			}

			var exportData = new ImportExportAllDataDto
			{
				Recipes = mapper.Map<List<RecipeDto>>(recipes.OrderBy(r => r.Name)),
				Ingredients = mapper.Map<List<IngredientDto>>(ingredients.OrderBy(i => i.Name)),
				RecipeIngredients = mapper.Map<List<RecipeIngredientDto>>(recipeIngredients),
				Users = usersWithRoles
			};

			var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
			{
				WriteIndented = true
			});

			return json;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to export all data");
			return Error.Failure("Failed to export all data");
		}
	}
}
