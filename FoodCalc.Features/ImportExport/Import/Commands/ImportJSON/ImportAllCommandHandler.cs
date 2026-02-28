
using AutoMapper;

using ErrorOr;

using FoodCalc.Features.ImportExport.Export.Commands.ExportJSON;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;
public class ImportAllCommandHandler(IUnitOfWork unitOfWork, ILogger<ImportAllCommandHandler> logger, IMapper mapper) : IRequestHandler<ImportAllCommand, ErrorOr<bool>>
{
	public async Task<ErrorOr<bool>> Handle(ImportAllCommand request, CancellationToken cancellationToken)
	{
        
        try
        {
            var data = request.Data;
            if (data == null)
            {
                return Error.Failure("No data provided for import.");
            }

			await unitOfWork.BeginTransactionAsync(cancellationToken);

			// Maps imported ingredient IDs to existing IDs when matched by name
			var ingredientIdRemap = new Dictionary<Guid, Guid>();

            // Import Ingredients
            foreach (var ingredientDto in data.Ingredients)
            {
                var existingById = await unitOfWork.IngredientRepository.GetByIdAsync(ingredientDto.Id, cancellationToken);
                if (existingById != null)
                {
                    bool changed = false;
                    if (existingById.Name != ingredientDto.Name)
                    {
                        existingById.Name = ingredientDto.Name;
                        changed = true;
                    }
                    if (existingById.ShouldBeAddedToShoppingCart != ingredientDto.ShouldBeAddedToShoppingCart)
                    {
                        existingById.ShouldBeAddedToShoppingCart = ingredientDto.ShouldBeAddedToShoppingCart;
                        changed = true;
                    }
                    if (changed)
                    {
                        await unitOfWork.IngredientRepository.UpdateAsync(existingById, cancellationToken);
                    }
                    continue;
                }

                var existingByName = await unitOfWork.IngredientRepository.GetByNameAsync(ingredientDto.Name, cancellationToken);
                if (existingByName != null)
                {
                    ingredientIdRemap[ingredientDto.Id] = existingByName.Id;
                    continue;
                }

                var ingredient = new Ingredient
                {
                    Id = ingredientDto.Id,
                    Name = ingredientDto.Name,
                    ShouldBeAddedToShoppingCart = ingredientDto.ShouldBeAddedToShoppingCart
                };
                await unitOfWork.IngredientRepository.AddAsync(ingredient, cancellationToken);
            }

            // Import Recipes
            foreach (var recipeDto in data.Recipes)
            {
                var existing = await unitOfWork.RecipeRepository.GetByIdAsync(recipeDto.Id, cancellationToken);
                if (existing != null)
                {
                    if (existing.Name != recipeDto.Name)
                    {
                        existing.Name = recipeDto.Name;
                        await unitOfWork.RecipeRepository.UpdateNameAsync(existing, cancellationToken);
                    }
                }
                else
                {
                    var recipe = new Recipe
                    {
                        Id = recipeDto.Id,
                        Name = recipeDto.Name,
                    };
                    await unitOfWork.RecipeRepository.AddAsync(recipe, cancellationToken);
                }
            }

            // Import RecipeIngredients
            foreach (RecipeIngredientDto riDto in data.RecipeIngredients)
            {
                var resolvedIngredientId = ingredientIdRemap.TryGetValue(riDto.IngredientId, out var remappedId)
                    ? remappedId
                    : riDto.IngredientId;

                var existingRi = await unitOfWork.RecipeRepository.GetRecipeIngredientByIdAsync(riDto.Id, cancellationToken);
                if (existingRi != null)
                {
                    bool changed = false;
                    if (existingRi.IngredientId != resolvedIngredientId)
                    {
                        existingRi.IngredientId = resolvedIngredientId;
                        changed = true;
                    }
                    if (existingRi.Amount != riDto.Amount)
                    {
                        existingRi.Amount = riDto.Amount;
                        changed = true;
                    }
                    var mappedAmountType = mapper.Map<IngredientAmountType>(riDto.IngredientAmount);
                    if (!Equals(existingRi.IngredientAmount, mappedAmountType))
                    {
                        existingRi.IngredientAmount = mappedAmountType;
                        changed = true;
                    }
                    if (changed)
                    {
                        await unitOfWork.RecipeRepository.UpdateRecipeIngredientAsync(existingRi, cancellationToken);
                    }
                    continue;
                }

                var recipeIngredient = new RecipeIngredient
                {
                    Id = riDto.Id,
                    RecipeId = riDto.RecipeId,
                    IngredientId = resolvedIngredientId,
                    Amount = riDto.Amount,
                    IngredientAmount = mapper.Map<IngredientAmountType>(riDto.IngredientAmount)
                };
                await unitOfWork.RecipeRepository.AddRecipeIngredientAsync(recipeIngredient, cancellationToken);
            }

            // Import Users with Roles (optional)
            if (data.Users != null)
            {
                foreach (UserWithRolesDto userDto in data.Users)
                {
                    var existing = await unitOfWork.UserRepository.GetByEmailAsync(userDto.Email, cancellationToken);
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
                        // You may need to set a default password or handle this elsewhere
                        await unitOfWork.UserRepository.UpdateAsync(user, cancellationToken);
                        // Add roles
                        foreach (var role in userDto.Roles)
                        {
                            await unitOfWork.UserRepository.AddRoleToUser(user, role, cancellationToken);
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
                    var existingRoles = await unitOfWork.UserRepository.GetRolesAsync(existing, cancellationToken);
                    var importedRoles = userDto.Roles?.ToHashSet() ?? new HashSet<string>();
                    var existingRolesSet = existingRoles?.ToHashSet() ?? new HashSet<string>();

                    var rolesToAdd = importedRoles.Except(existingRolesSet).ToList();
                    var rolesToRemove = existingRolesSet.Except(importedRoles).ToList();

                    if (rolesToAdd.Count > 0 || rolesToRemove.Count > 0)
                    {
                        changed = true;
                        foreach (var role in rolesToAdd)
                        {
                            await unitOfWork.UserRepository.AddRoleToUser(existing, role, cancellationToken);
                        }
                        foreach (var role in rolesToRemove)
                        {
                            await unitOfWork.UserRepository.RemoveRoleFromUser(existing, role, cancellationToken);
                        }
                    }

                    if (changed)
                    {
                        await unitOfWork.UserRepository.UpdateAsync(existing, cancellationToken);
                    }
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            logger.LogError(ex, "Failed to import all data");
            return Error.Failure("Failed to import all data");
        }
	}
}
