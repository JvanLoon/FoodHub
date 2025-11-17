using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.DTOs;

public class UserWithRolesDto
{
	public string Id { get; set; } = default!;
	public string Email { get; set; } = default!;
	public bool LockoutEnabled { get; set; }
	public bool EmailConfirmed { get; set; }
	public List<string> Roles { get; set; } = [];
}
