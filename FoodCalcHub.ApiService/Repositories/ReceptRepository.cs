using FoodCalcHub.ApiService.Entities;
using FoodCalcHub.ApiService.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FoodCalcHub.ApiService.Repositories;

public class ReceptRepository(ApplicationDbContext context) : IReceptRepository
{
    public async Task<List<Recept>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Recepts.ToListAsync();
    }

    public async Task<Recept?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Recepts.FindAsync(id);
    }

    public async Task<Recept> AddAsync(Recept recept, CancellationToken cancellationToken)
    {
        context.Recepts.Add(recept);
        await context.SaveChangesAsync();
        return recept;
    }

    public async Task UpdateAsync(Recept recept, CancellationToken cancellationToken)
    {
        context.Entry(recept).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var recept = await context.Recepts.FindAsync(id);
        if (recept != null)
        {
            context.Recepts.Remove(recept);
            await context.SaveChangesAsync();
        }
    }
}
