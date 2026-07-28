using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace FoodCalc.Api.Endpoints.Admin;

public class GetUserRolesRequest
{
	[BindFrom("email")]
	public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Query-bound twin of <see cref="ModifyUserRoleRequest"/>, for the DELETE. A DELETE has no
/// body, so this one cannot move to FoodHub.DTOs without taking a FastEndpoints reference
/// with it — the attributes below are what bind it.
/// </summary>
public class RemoveUserRoleRequest
{
	[BindFrom("email")]
	public string Email { get; set; } = string.Empty;

	[BindFrom("role")]
	public string Role { get; set; } = string.Empty;
}

/// <summary>GET api/admin/userroles?email= — Admin or Moderator.</summary>
public class GetUserRolesEndpoint(UserManager<IdentityUser> userManager) : Endpoint<GetUserRolesRequest>
{
	public override void Configure()
	{
		Get(ApiRoutes.Admin.UserRoles);
		Policies("Admin,Moderator");
	}

	public override async Task HandleAsync(GetUserRolesRequest req, CancellationToken ct)
	{
		var user = await userManager.FindByEmailAsync(req.Email);
		if (user == null)
		{
			await Send.StringAsync("No user found", 401, cancellation: ct);
			return;
		}

		await Send.OkAsync(await userManager.GetRolesAsync(user), ct);
	}
}

/// <summary>POST api/admin/userroles — Admin or Moderator. Takes a JSON body.</summary>
public class AddUserRoleEndpoint(UserManager<IdentityUser> userManager) : Endpoint<ModifyUserRoleRequest>
{
	public override void Configure()
	{
		Post(ApiRoutes.Admin.UserRoles);
		Policies("Admin,Moderator");
	}

	public override async Task HandleAsync(ModifyUserRoleRequest req, CancellationToken ct)
	{
		var user = await userManager.FindByEmailAsync(req.Email);
		if (user == null)
		{
			await Send.StringAsync("No user found", 404, cancellation: ct);
			return;
		}

		var result = await userManager.AddToRoleAsync(user, req.Role);
		if (!result.Succeeded)
		{
			await this.SendErrorsAsync(result.Errors.Select(e => e.Description), ct: ct);
			return;
		}

		await Send.OkAsync(ct);
	}
}

/// <summary>DELETE api/admin/userroles?email=&amp;role= — Admin or Moderator.</summary>
public class RemoveUserRoleEndpoint(UserManager<IdentityUser> userManager) : Endpoint<RemoveUserRoleRequest>
{
	public override void Configure()
	{
		Delete(ApiRoutes.Admin.UserRoles);
		Policies("Admin,Moderator");
	}

	public override async Task HandleAsync(RemoveUserRoleRequest req, CancellationToken ct)
	{
		var user = await userManager.FindByEmailAsync(req.Email);
		if (user == null)
		{
			await Send.StringAsync("No user found", 404, cancellation: ct);
			return;
		}

		var result = await userManager.RemoveFromRoleAsync(user, req.Role);
		if (!result.Succeeded)
		{
			await this.SendErrorsAsync(result.Errors.Select(e => e.Description), ct: ct);
			return;
		}

		await Send.OkAsync(ct);
	}
}