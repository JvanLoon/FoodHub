using FastEndpoints;

using FluentValidation;

using FoodHub.DTOs;

namespace FoodCalc.Api.Endpoints.Authentication;

/// <summary>
/// Mirrors the DataAnnotations on RegisterDto ([Required][EmailAddress] Email,
/// [Required][StringLength(100, MinimumLength = 6)] Password). The old
/// AuthenticationController was marked [ApiController], so these were enforced
/// automatically via ModelState; FastEndpoints only runs FluentValidation, so
/// the rules are restated here to preserve the 400-on-invalid behavior.
/// </summary>
public class RegisterValidator : Validator<RegisterDto>
{
	public RegisterValidator()
	{
		RuleFor(x => x.Email).NotEmpty().EmailAddress();
		RuleFor(x => x.Password).NotEmpty().Length(6, 100);
	}
}

/// <summary>Mirrors the DataAnnotations on ResetPasswordDto (see RegisterValidator).</summary>
public class ResetPasswordValidator : Validator<ResetPasswordDto>
{
	public ResetPasswordValidator()
	{
		RuleFor(x => x.Email).NotEmpty().EmailAddress();
		RuleFor(x => x.Password).NotEmpty().Length(6, 100);
	}
}

/// <summary>Hardening: reject blank credentials before hitting the user store.</summary>
public class LoginValidator : Validator<LoginDto>
{
	public LoginValidator()
	{
		RuleFor(x => x.Email).NotEmpty();
		RuleFor(x => x.Password).NotEmpty();
	}
}

/// <summary>Hardening: require an email for the toggle-user query.</summary>
public class ToggleUserRequestValidator : Validator<ToggleUserRequest>
{
	public ToggleUserRequestValidator()
	{
		RuleFor(x => x.Email).NotEmpty();
	}
}
