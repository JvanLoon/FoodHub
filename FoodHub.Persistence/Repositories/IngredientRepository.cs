using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Persistence.Repositories;

public class IngredientRepository(ApplicationDbContext context) : IIngredientRepository
{
    public async Task<IEnumerable<Ingredient>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Ingredients.ToListAsync();
    }

    public async Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Ingredients.FindAsync(id);
    }

    public async Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken)
    {
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken)
    {
        context.Entry(ingredient).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var ingredient = await context.Ingredients.FindAsync(id);
        if (ingredient != null)
        {
            context.Ingredients.Remove(ingredient);
            await context.SaveChangesAsync();
        }
    }
}
