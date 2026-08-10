using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;

public class ImportAllCommandHandler(
    FoodHubDbContext context,
    UserManager<IdentityUser> userManager,
    ILogger<ImportAllCommandHandler> logger) : IRequestHandler<ImportAllCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(ImportAllCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var data = request.Data;
            if (data == null) { return Error.Failure(description: ErrorMessages.ImportExport.NoImportData); }

            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            // Import Ingredients (catalog)
            foreach (var ingredientDto in data.Ingredients)
            {
                var existingById = context.Ingredients.Local.SingleOrDefault(ri => ri.Id == ingredientDto.Id);
                if (existingById != null)
                {
                    existingById = await context.Ingredients
                        .SingleOrDefaultAsync(i => i.Id == ingredientDto.Id, cancellationToken);
                }
                
                if (existingById != null)
                {
                    if (existingById.Name != ingredientDto.Name)
                        existingById.Name = ingredientDto.Name;

                    if (existingById.ShouldBeAddedToShoppingCart != ingredientDto.ShouldBeAddedToShoppingCart)
                    {
                        existingById.ShouldBeAddedToShoppingCart = ingredientDto.ShouldBeAddedToShoppingCart;
                    }

                    continue;
                }

                context.Ingredients.Add(new Ingredient
                {
                    Id = ingredientDto.Id,
                    Name = ingredientDto.Name,
                    ShouldBeAddedToShoppingCart = ingredientDto.ShouldBeAddedToShoppingCart,
                    CreatedByUserId = request.ImportedByUserId,
                    // Imported rows arrive approved. Import is an Admin-only restore of curated
                    // data, not a submission, and the alternative is worse than untidy: an
                    // unapproved row is invisible to everyone but its author, so a restored
                    // backup would silently come back empty for the whole user base.
                    IsReviewed = true,
                    FirstApprovedDate = DateTime.UtcNow
                });
            }

            // Import Recipes
            foreach (var recipeDto in data.Recipes)
            {
                var existing = context.Recipes.Local.SingleOrDefault(ri => ri.Id == recipeDto.Id);
                if (existing != null)
                {
                    existing = await context.Recipes.SingleOrDefaultAsync(r => r.Id == recipeDto.Id, cancellationToken);
                }
                
                
                if (existing != null)
                {
                    if (existing.Name != recipeDto.Name)
                    {
                        existing.Name = recipeDto.Name;
                    }
                }
                
                context.Recipes.Add(new Recipe
                {
                    Id = recipeDto.Id,
                    Name = recipeDto.Name,
                    CreatedByUserId = request.ImportedByUserId,
                    // Approved on arrival — see the note on the ingredient import above.
                    IsReviewed = true,
                    FirstApprovedDate = DateTime.UtcNow
                });
            }

            // Import RecipeItems (ingredient lines snapshotted onto the recipe)
            foreach (RecipeItemDto riDto in data.RecipeItems)
            {
                
                
                var existingRi = context.RecipeItems.Local.SingleOrDefault(ri => ri.Id == riDto.Id);
                if (existingRi != null)
                {
                    existingRi = await context.RecipeItems.SingleOrDefaultAsync(ri => ri.Id == riDto.Id, cancellationToken);
                }
                
                if (existingRi != null)
                {
                    existingRi.Ingredient = existingRi.Ingredient;
                    existingRi.Amount = riDto.Amount;
                    existingRi.IngredientAmount = (IngredientAmountType) riDto.IngredientAmount;
                    existingRi.ShouldBeAddedToShoppingCart = riDto.ShouldBeAddedToShoppingCart;
                    continue;
                }
                
                var ingredientEntity = context.Ingredients.Local.SingleOrDefault(i => i.Id == riDto.IngredientId);
                if (ingredientEntity != null)
                {
                    ingredientEntity = await context.Ingredients.SingleOrDefaultAsync(i => i.Id == riDto.IngredientId, cancellationToken);
                }

                if (ingredientEntity != null)
                {
                    context.RecipeItems.Add(new RecipeItem
                    {
                        Id = riDto.Id,
                        RecipeId = riDto.RecipeId,
                        Ingredient = ingredientEntity,
                        Amount = riDto.Amount,
                        IngredientAmount = (IngredientAmountType) riDto.IngredientAmount,
                        ShouldBeAddedToShoppingCart = riDto.ShouldBeAddedToShoppingCart,
                        // Reviewed, so the restored recipe does not read as wholly rewritten the
                        // first time someone edits it.
                        IsReviewed = riDto.IsReviewed
                    });
                }
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
                            UserName = userDto.UserName,
                            Email = userDto.Email,
                            EmailConfirmed = userDto.EmailConfirmed,
                            PasswordHash = userDto.PasswordHash,
                            SecurityStamp = userDto.SecurityStamp,
                            ConcurrencyStamp = userDto.ConcurrencyStamp,
                            LockoutEnd = userDto.LockoutEnd,
                            LockoutEnabled = userDto.LockoutEnabled,
                            AccessFailedCount = userDto.AccessFailedCount,
                        };

                        await userManager.CreateAsync(user);
                        
                        //todo make sure all roles are there. keep in mind, saftyrisk. not when u only add roles already in db
                        if (userDto.Roles != null && userDto.Roles.Count != 0)
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

                    // Same for an account that already exists: the import repairs LockoutEnabled
                    // towards the invariant rather than towards whatever the file says.
                    if (!existing.LockoutEnabled)
                    {
                        existing.LockoutEnabled = true;
                        changed = true;
                    }

                    // Sync roles
                    var existingRoles = await userManager.GetRolesAsync(existing);
                    var importedRoles = userDto.Roles?.ToHashSet() ?? [];
                    var existingRolesSet = existingRoles.ToHashSet();

                    var rolesToAdd = importedRoles.Except(existingRolesSet)
                        .ToList();
                    var rolesToRemove = existingRolesSet.Except(importedRoles)
                        .ToList();

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
            return Error.Failure(description: ErrorMessages.ImportExport.ImportFailed);
        }
    }
}