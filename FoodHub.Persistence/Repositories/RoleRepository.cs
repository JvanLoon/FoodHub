using FoodHub.Persistence.Repositories.Interface;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.Persistence.Repositories
{
	public class RoleRepository(ApplicationDbContext context) : IRoleRepository
	{
		public Task<List<string>> GetAllAsync(CancellationToken cancellationToken)
		{
			return context.Roles.Select(r => r.Name!).ToListAsync(cancellationToken);
		}
	}
}
