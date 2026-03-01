namespace FoodHub.Persistence.Repositories
{
	public class RoleRepository(ApplicationDbContext context)
	{
        public IQueryable<string> GetAllAsync()
        {
            return context.Roles.Select(r => r.Name!).AsQueryable();
        }
	}
}
