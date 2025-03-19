using Azure.Core;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories.Interface;

using Microsoft.EntityFrameworkCore;

namespace FoodHub.Persistence.Repositories;

public class ReceptRepository(ApplicationDbContext context) : IReceptRepository
{
    public async Task<List<Recept>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Recepts.ToListAsync(cancellationToken);
    }

    public async Task<Recept?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
		return await context.Recepts.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Recept> AddAsync(Recept recept, CancellationToken cancellationToken)
    {
        context.Recepts.Add(recept);

		await context.SaveChangesAsync(cancellationToken);
        return recept;
    }

    public async Task UpdateAsync(Recept recept, CancellationToken cancellationToken)
    {
		// Attach the Recept entity to the context
		context.Recepts.Attach(recept);

		// Mark the Recept entity as modified
		context.Entry(recept).State = EntityState.Modified;

		// Update the related ReceptIngredient entities
		foreach (var receptIngredient in recept.ReceptIngredient)
		{
			// Attach the ReceptIngredient entity to the context
			context.ReceptIngredients.Attach(receptIngredient);
			
			// Mark the ReceptIngredient entity as modified
			context.Entry(receptIngredient).State = EntityState.Modified;
		}

		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Recept? recept = await context.Recepts.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (recept != null)
        {
            context.Recepts.Remove(recept);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

	public async Task AddReceptIngredientAsync(ReceptIngredient receptIngredient, CancellationToken cancellationToken)
	{
		context.ReceptIngredients.Add(receptIngredient);

		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateReceptIngredientAsync(ReceptIngredient receptIngredient, CancellationToken cancellationToken)
	{
		var existingEntity = await context.ReceptIngredients
			.FirstOrDefaultAsync(ri => ri.Id == receptIngredient.Id, cancellationToken);

		// Entity exists, update it
		context.Entry(existingEntity).CurrentValues.SetValues(receptIngredient);

		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteReceptIngredientAsync(Guid receptIngredientId, CancellationToken cancellationToken)
	{
		ReceptIngredient receptIngredient = 
			await context.ReceptIngredients.SingleOrDefaultAsync(r => r.Id == receptIngredientId, cancellationToken);

		if (receptIngredient != null)
		{
			context.ReceptIngredients.Remove(receptIngredient);
			await context.SaveChangesAsync(cancellationToken);
		}
	}
}
