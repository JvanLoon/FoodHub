using FoodHub.Persistence.Entities;

namespace FoodHub.Persistence.Repositories.Interface;
public interface IReceptRepository
{
    Task<List<Recept>> GetAllAsync(CancellationToken cancellationToken);
    Task<Recept?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Recept> AddAsync(Recept recept, CancellationToken cancellationToken);
    Task UpdateAsync(Recept recept, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
