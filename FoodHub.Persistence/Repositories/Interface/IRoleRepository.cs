using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.Persistence.Repositories.Interface;

public interface IRoleRepository
{
	Task<List<string>> GetAllAsync(CancellationToken cancellationToken);
}
