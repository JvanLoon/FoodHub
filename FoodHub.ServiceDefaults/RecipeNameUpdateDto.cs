using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.ServiceDefaults
{
	public class RecipeNameUpdateDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}
}
