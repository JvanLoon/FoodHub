using FastEndpoints;

using Microsoft.AspNetCore.Identity;

namespace FoodCalc.Api.Endpoints.Authentication;

public class ToggleUserRequest
{
	[BindFrom("email")]
	public string Email { get; set; } = string.Empty;

	[BindFrom("enable")]
	public bool Enable { get; set; } = true;
}

/// <summary>POST api/authentication/toggleUser?email=&amp;enable= — Admin.</summary>
public class ToggleUserEndpoint(UserManager<IdentityUser> userManager)
	: Endpoint<ToggleUserRequest>
{
	public override void Configure()
	{
		Post("api/authentication/toggleUser");
		Policies("Admin");
	}

	public override async Task HandleAsync(ToggleUserRequest req, CancellationToken ct)
	{
		var user = await userManager.FindByEmailAsync(req.Email);
		if (user == null)
		{
			await Send.StringAsync("User not found", 404, cancellation: ct);
			return;
		}

		user.EmailConfirmed = req.Enable;

		if (!await userManager.IsInRoleAsync(user, "User"))
		{
			await userManager.AddToRoleAsync(user, "User");
		}

		var result = await userManager.UpdateAsync(user);
		if (!result.Succeeded)
		{
			await Send.ResultAsync(TypedResults.BadRequest(result.Errors));
			return;
		}

		await Send.OkAsync(ct);
	}
}
