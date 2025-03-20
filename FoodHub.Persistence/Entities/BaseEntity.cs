using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.Persistence.Entities;
public abstract class BaseEntity
{
	public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
	public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
}
