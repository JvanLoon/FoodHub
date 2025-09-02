using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.DTOs;
public class BlackListDto
{
	public Guid UserId { get; set; }
	public Guid RecipeId { get; set; }
}
