using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.DTOs;
public class UserDto
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public bool Enabled { get; set; }
	public List<string> Roles { get; set; }
}
