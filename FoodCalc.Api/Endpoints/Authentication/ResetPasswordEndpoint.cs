using FastEndpoints;

using Microsoft.AspNetCore.Identity;

namespace FoodCalc.Api.Endpoints.Authentication;

/// <summary>POST api/authentication/resetpassword — Admin.</summary>
public class ResetPasswordEndpoint(UserManager<IdentityUser> userManager)
	: Endpoint<ResetPasswordDto>
{
	public override void Configure()
	{
		Post(ApiRoutes.Authentication.ResetPassword);
		Policies("Admin");
	}

	public override async Task HandleAsync(ResetPasswordDto req, CancellationToken ct)
	{
		var user = await userManager.FindByEmailAsync(req.Email);
		if (user == null)
		{
			await Send.StringAsync("User not found", 404, cancellation: ct);
			return;
		}

		var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

		var result = await userManager.ResetPasswordAsync(user, resetToken, req.Password);
		if (!result.Succeeded)
		{
			await Send.ResultAsync(TypedResults.BadRequest(result.Errors.Select(e => e.Description)));
			return;
		}

		await Send.StringAsync("Password has been reset successfully.", cancellation: ct);
	}
}
