namespace FoodHub.DTOs;
public class UserDto
{
	public string Id { get; set; } = null!;
	public string Name { get; set; } = null!;
	public string Email { get; set; } = null!;
	public bool Enabled { get; set; }
	public bool EmailConfirmed { get; set; }
	public List<string> Roles { get; set; } = [];
}
