using System.ComponentModel.DataAnnotations;
using FoodHub.DTOs.Constants;

namespace FoodHub.DTOs;

public class ResetPasswordDto
{
    [Required(ErrorMessage = DtoValidationMessages.EmailRequired)]
    [EmailAddress(ErrorMessage = DtoValidationMessages.EmailInvalid)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = DtoValidationMessages.PasswordRequired)]
    [StringLength(DtoValidationMessages.PasswordMaxLength,
        MinimumLength = DtoValidationMessages.PasswordMinLength,
        ErrorMessage = DtoValidationMessages.PasswordLength)]
    public string Password { get; set; } = string.Empty;
}