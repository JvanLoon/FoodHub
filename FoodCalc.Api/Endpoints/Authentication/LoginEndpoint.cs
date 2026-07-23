using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodCalc.Api.Endpoints.Authentication;

/// <summary>POST api/authentication/login — anonymous. Returns a JWT + email.</summary>
public class LoginEndpoint(
	IConfiguration configuration,
	UserManager<IdentityUser> userManager,
	SignInManager<IdentityUser> signInManager) : Endpoint<LoginDto, AuthResponseDto>
{
	public override void Configure()
	{
		Post(ApiRoutes.Authentication.Login);
		AllowAnonymous();
	}

	public override async Task HandleAsync(LoginDto req, CancellationToken ct)
	{
		var user = await userManager.FindByEmailAsync(req.Email);
		if (user == null)
		{
			await Send.StringAsync("No user found", 401, cancellation: ct);
			return;
		}

		if (!user.EmailConfirmed)
		{
			await Send.StringAsync("Email not confirmed", 401, cancellation: ct);
			return;
		}

		if (user.LockoutEnabled)
		{
			if (user.LockoutEnd < DateTime.Now) { await userManager.SetLockoutEnabledAsync(user, false); }
			else
			{
				await Send.StringAsync("User Lockedout", 401, cancellation: ct);
				return;
			}
		}

		var result = await signInManager.CheckPasswordSignInAsync(user, req.Password, false);
		if (!result.Succeeded)
		{
			await userManager.AccessFailedAsync(user);
			await Send.StringAsync("Invalid password", 401, cancellation: ct);
			return;
		}

		List<Claim> claims =
		[
			new Claim(JwtRegisteredClaimNames.Sub, user.Id), new Claim(JwtRegisteredClaimNames.Email, user.Email!),
		];

		var roles = await userManager.GetRolesAsync(user);
		foreach (var role in roles) { claims.Add(new Claim(ClaimTypes.Role, role)); }

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var token = new JwtSecurityToken(issuer: configuration["Jwt:Issuer"], claims: claims,
										 expires: DateTime.UtcNow.AddHours(12), signingCredentials: creds);

		var response = new AuthResponseDto
		{
			Token = new JwtSecurityTokenHandler().WriteToken(token), Email = user.Email!,
		};

		await Send.OkAsync(response, ct);
	}
}