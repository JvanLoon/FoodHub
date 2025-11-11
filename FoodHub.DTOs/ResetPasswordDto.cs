using System.ComponentModel.DataAnnotations;

namespace FoodHub.DTOs;
public class ResetPasswordDto
{
	[Required]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	[Required]
	[StringLength(100, MinimumLength = 6)]
	public string Password { get; set; } = string.Empty;
}
