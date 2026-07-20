
using ErrorOr;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;
public class ImportAllCommandHandler(FoodHubDbContext context, UserManager<IdentityUser> userManager, ILogger<ImportAllCommandHandler> logger) : IRequestHandler<ImportAllCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(ImportAllCommand request, CancellationToken cancellationToken)
	{
        try
        {
            var data = request.Data;
            if (data == null)
            {
                return Error.Failure(ErrorMessages.ImportExport.NoImportData);
            }

			await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            // Import Ingredients (catalog)
            foreach (var ingredientDto in data.Ingredients)
            {
                var existingById = await context.Ingredients.SingleOrDefaultAsync(i => i.Id == ingredientDto.Id, cancellationToken);
                if (existingById != null)
                {
                    if (existingById.Name != ingredientDto.Name)
                    {
                        existingById.Name = ingredientDto.Name;
                    }
                    if (existingById.ShouldBeAddedToShoppingCart != ingredientDto.ShouldBeAddedToShoppingCart)
                    {
                        existingById.ShouldBeAddedToShoppingCart = ingredientDto.ShouldBeAddedToShoppingCart;
                    }
                    continue;
                }

                var existingByName = await context.Ingredients.SingleOrDefaultAsync(i => i.Name == ingredientDto.Name, cancellationToken);
                if (existingByName != null)
                {
                    continue;
                }

                context.Ingredients.Add(new Ingredient
                {
                    Id = ingredientDto.Id,
                    Name = ingredientDto.Name,
                    ShouldBeAddedToShoppingCart = ingredientDto.ShouldBeAddedToShoppingCart
                });
            }

            // Import Recipes
            foreach (var recipeDto in data.Recipes)
            {
                var existing = await context.Recipes.SingleOrDefaultAsync(r => r.Id == recipeDto.Id, cancellationToken);
                if (existing != null)
                {
                    if (existing.Name != recipeDto.Name)
                    {
                        existing.Name = recipeDto.Name;
                    }
                }
                else
                {
                    context.Recipes.Add(new Recipe
                    {
                        Id = recipeDto.Id,
                        Name = recipeDto.Name,
                    });
                }
            }

            // Import RecipeIngredients (ingredient lines snapshotted onto the recipe)
            foreach (RecipeIngredientDto riDto in data.RecipeIngredients)
            {
                var existingRi = await context.RecipeIngredients.SingleOrDefaultAsync(ri => ri.Id == riDto.Id, cancellationToken);
                if (existingRi != null)
                {
                    existingRi.Name = riDto.Name;
                    existingRi.Amount = riDto.Amount;
                    existingRi.IngredientAmount = (IngredientAmountType)riDto.IngredientAmount;
                    existingRi.ShouldBeAddedToShoppingCart = riDto.ShouldBeAddedToShoppingCart;
                    continue;
                }

                context.RecipeIngredients.Add(new RecipeIngredient
                {
                    Id = riDto.Id,
                    RecipeId = riDto.RecipeId,
                    Name = riDto.Name,
                    Amount = riDto.Amount,
                    IngredientAmount = (IngredientAmountType)riDto.IngredientAmount,
                    ShouldBeAddedToShoppingCart = riDto.ShouldBeAddedToShoppingCart
                });
            }

            // Import Users with Roles (optional)
            if (data.Users != null)
            {
                foreach (UserWithRolesDto userDto in data.Users)
                {
                    var existing = await userManager.FindByEmailAsync(userDto.Email);
                    if (existing == null)
                    {
                        var user = new IdentityUser
                        {
                            Id = userDto.Id,
                            Email = userDto.Email,
                            UserName = userDto.Email,
                            EmailConfirmed = userDto.EmailConfirmed,
                            LockoutEnabled = userDto.LockoutEnabled
                        };
                        await userManager.CreateAsync(user);
                        if (userDto.Roles != null)
                        {
                            foreach (var role in userDto.Roles)
                            {
                                await userManager.AddToRoleAsync(user, role);
                            }
                        }
                        continue;
                    }

                    bool changed = false;
                    if (existing.EmailConfirmed != userDto.EmailConfirmed)
                    {
                        existing.EmailConfirmed = userDto.EmailConfirmed;
                        changed = true;
                    }
                    if (existing.LockoutEnabled != userDto.LockoutEnabled)
                    {
                        existing.LockoutEnabled = userDto.LockoutEnabled;
                        changed = true;
                    }

                    // Sync roles
                    var existingRoles = await userManager.GetRolesAsync(existing);
                    var importedRoles = userDto.Roles?.ToHashSet() ?? new HashSet<string>();
                    var existingRolesSet = existingRoles.ToHashSet();

                    var rolesToAdd = importedRoles.Except(existingRolesSet).ToList();
                    var rolesToRemove = existingRolesSet.Except(importedRoles).ToList();

                    if (rolesToAdd.Count > 0)
                        await userManager.AddToRolesAsync(existing, rolesToAdd);
                    if (rolesToRemove.Count > 0)
                        await userManager.RemoveFromRolesAsync(existing, rolesToRemove);

                    if (changed)
                        await userManager.UpdateAsync(existing);
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.ImportExport.ImportFailed);
            return Error.Failure(ErrorMessages.ImportExport.ImportFailed);
        }
	}
}
