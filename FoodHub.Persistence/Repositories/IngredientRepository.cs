using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Persistence.Repositories;

public class IngredientRepository(ApplicationDbContext context) : IIngredientRepository
{
    public async Task<List<Ingredient>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Ingredients.ToListAsync(cancellationToken);
    }

    public async Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Ingredients.SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken)
    {
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken)
    {
        context.Entry(ingredient).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Ingredient? ingredient = await context.Ingredients.SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (ingredient != null)
        {
            context.Ingredients.Remove(ingredient);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
