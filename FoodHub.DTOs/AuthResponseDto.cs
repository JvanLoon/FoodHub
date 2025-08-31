namespace FoodHub.DTOs;
public class AuthResponseDto
{
	public string Token { get; set; }
	public string Email { get; set; }
	public bool Enabled { get; set; }
}
