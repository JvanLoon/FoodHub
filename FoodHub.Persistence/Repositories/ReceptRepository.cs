using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Persistence.Repositories;

public class ReceptRepository(ApplicationDbContext context) : IReceptRepository
{
    public async Task<List<Recept>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Recepts.Include(r => r.ReceptIngredient).ToListAsync(cancellationToken);
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
        context.Entry(recept).State = EntityState.Modified;

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
}
