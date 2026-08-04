using FastEndpoints;
using FoodCalc.Features.Authentication.Presence.Commands.TouchPresence;
using MediatR;
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
    SignInManager<IdentityUser> signInManager,
    IMediator mediator) : Endpoint<LoginDto, AuthResponseDto>
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
            await Send.StringAsync(ResponseMessages.Account.UserNotFound, 401, cancellation: ct);
            return;
        }

        if (!user.EmailConfirmed)
        {
            await Send.StringAsync(ResponseMessages.Account.EmailNotConfirmed, 401, cancellation: ct);
            return;
        }

        if (user.LockoutEnabled)
        {
            if (user.LockoutEnd < DateTime.Now) { await userManager.SetLockoutEnabledAsync(user, false); }
            else
            {
                await Send.StringAsync(ResponseMessages.Account.UserLockedOut, 401, cancellation: ct);
                return;
            }
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!result.Succeeded)
        {
            await userManager.AccessFailedAsync(user);
            await Send.StringAsync(ResponseMessages.Account.InvalidPassword, 401, cancellation: ct);
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

        // This route is anonymous, so PresenceMiddleware has no principal to read and skips it.
        // Marking the user online here means an admin sees the dot at once, rather than after the
        // client's first heartbeat a minute later.
        await mediator.Send(new TouchPresenceCommand(user.Id), ct);

        var response = new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Email = user.Email!,
        };

        await Send.OkAsync(response, ct);
    }
}