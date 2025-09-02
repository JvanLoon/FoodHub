using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.Persistence.Entities;
public class RecipeBlackList
{
	public Guid RecipeId { get; set; }
	public Guid UserId { get; set; }
}
