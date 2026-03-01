using FoodHub.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace FoodHub.Persistence.Repositories;

public class RecipeRepository(ApplicationDbContext context)
{
    public IQueryable<Recipe> GetAllAsync()
    {
        return context.Recipes.AsQueryable();
    }

    public async Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
		return await context.Recipes.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

	public async Task<RecipeIngredient?> GetIngredientRecipeByRecipeId(Guid RecipeId, Guid IngredientRecipeId, CancellationToken cancellationToken)
	{
		Recipe? recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == RecipeId, cancellationToken);

		if (recipe == null)
		{
			throw new ArgumentNullException(nameof(recipe));
		}

		return recipe.RecipeIngredient.Where(re => re.Id == IngredientRecipeId).FirstOrDefault();
	}

	public async Task<Recipe> AddAsync(Recipe recipe, CancellationToken cancellationToken)
    {
        context.Recipes.Add(recipe);

		await context.SaveChangesAsync(cancellationToken);
        return recipe;
    }

    public async Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken)
    {
		// Attach the Recipe entity to the context
		context.Recipes.Attach(recipe);

		// Mark the Recipe entity as modified
		context.Entry(recipe).State = EntityState.Modified;

		// Update the related RecipeIngredient entities
		foreach (var recipeIngredient in recipe.RecipeIngredient)
		{
			// Attach the RecipeIngredient entity to the context
			context.RecipeIngredients.Attach(recipeIngredient);
			
			// Mark the RecipeIngredient entity as modified
			context.Entry(recipeIngredient).State = EntityState.Modified;
		}

		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateNameAsync(Recipe recipe, CancellationToken cancellationToken)
	{
		// Attach the Recipe entity to the context
		context.Recipes.Attach(recipe);

		// Mark the Recipe entity as modified
		context.Entry(recipe).State = EntityState.Modified;

		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Recipe? recipe = await context.Recipes.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (recipe != null)
        {
            context.Recipes.Remove(recipe);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

	public async Task<RecipeIngredient?> GetRecipeIngredientByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await context.RecipeIngredients.SingleOrDefaultAsync(ri => ri.Id == id, cancellationToken);
	}

	public async Task AddRecipeIngredientAsync(RecipeIngredient recipeIngredient, CancellationToken cancellationToken)
	{
		context.RecipeIngredients.Add(recipeIngredient);

		await context.SaveChangesAsync(cancellationToken);
	}

    public async Task UpdateRecipeIngredientAsync(RecipeIngredient recipeIngredient, CancellationToken cancellationToken)
    {
        // Ensure the entity is not null before calling Entry
        if (recipeIngredient == null)
        {
            throw new ArgumentNullException(nameof(recipeIngredient));
        }

        RecipeIngredient? existingEntity = await context.RecipeIngredients
            .FirstOrDefaultAsync(ri => ri.Id == recipeIngredient.Id, cancellationToken);

        // Ensure the existing entity is not null before updating
        if (existingEntity == null)
        {
            throw new InvalidOperationException($"RecipeIngredient with ID {recipeIngredient.Id} not found.");
        }

        // Update the existing entity's values
        context.Entry(existingEntity).CurrentValues.SetValues(recipeIngredient);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRecipeIngredientAsync(Guid recipeIngredientId, CancellationToken cancellationToken)
    {
        RecipeIngredient? recipeIngredient = await context.RecipeIngredients.SingleOrDefaultAsync(r => r.Id == recipeIngredientId, cancellationToken);

        if (recipeIngredient != null)
        {
            context.RecipeIngredients.Remove(recipeIngredient);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
