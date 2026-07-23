using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FoodCalc.Features.ImportExport.Export.Commands.ExportJSON;

public class ExportAllCommandHandler(
	FoodHubDbContext context,
	UserManager<IdentityUser> userManager,
	ILogger<ExportAllCommandHandler> logger) : IRequestHandler<ExportAllCommand, ErrorOr<string>>
{
	public async Task<ErrorOr<string>> Handle(ExportAllCommand request, CancellationToken cancellationToken)
	{
		try
		{
			// Fetch all data (Recipe.Ingredients is auto-included)
			var recipes = await context.Recipes.ToListAsync(cancellationToken);
			var ingredients = await context.Ingredients.ToListAsync(cancellationToken);

			// Flatten the recipe ingredient lines across all recipes
			var recipeItems = new List<RecipeItem>();
			foreach (var recipe in recipes)
			{
				if (recipe.Ingredients != null && recipe.Ingredients.Count > 0)
				{
					recipeItems.AddRange(recipe.Ingredients);
				}
			}

			// Optionally fetch users with roles
			List<UserWithRolesDto>? usersWithRoles = null;
			if (request.includeUsers)
			{
				var users = await context.Users.ToListAsync(cancellationToken);
				usersWithRoles = [];
				foreach (var user in users)
				{
					var roles = await userManager.GetRolesAsync(user);
					usersWithRoles.Add(new UserWithRolesDto
					{
						Id = user.Id,
						Email = user.Email ?? "",
						EmailConfirmed = user.EmailConfirmed,
						LockoutEnabled = user.LockoutEnabled,
						Roles = roles.ToList()
					});
				}
			}

			var exportData = new ImportExportAllDataDto
			{
				Recipes = recipes.OrderBy(r => r.Name)
								 .ToDtoList(),
				Ingredients = ingredients.OrderBy(i => i.Name)
										 .ToDtoList(),
				RecipeItems = recipeItems.ToDtoList(),
				Users = usersWithRoles
			};

			var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions {WriteIndented = true});

			return json;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.ImportExport.ExportFailed);
			return Error.Failure(description: ErrorMessages.ImportExport.ExportFailed);
		}
	}
}