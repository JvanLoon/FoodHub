using FastEndpoints;

using FoodHub.DTOs;

using Microsoft.AspNetCore.Identity;

namespace FoodCalc.Api.Endpoints.Authentication;

/// <summary>POST api/authentication/register — anonymous.</summary>
public class RegisterEndpoint(UserManager<IdentityUser> userManager)
	: Endpoint<RegisterDto>
{
	public override void Configure()
	{
		Post(ApiRoutes.Authentication.Register);
		AllowAnonymous();
	}

	public override async Task HandleAsync(RegisterDto req, CancellationToken ct)
	{
		var user = new IdentityUser { UserName = req.Email, Email = req.Email };
		var result = await userManager.CreateAsync(user, req.Password);
		if (!result.Succeeded)
		{
			// Raw text/plain body: the Blazor LoginService reads it via
			// ReadAsStringAsync and shows it as the error message.
			await Send.StringAsync(result.Errors.First().Description, 400, cancellation: ct);
			return;
		}

		await Send.OkAsync(ct);
	}
}
