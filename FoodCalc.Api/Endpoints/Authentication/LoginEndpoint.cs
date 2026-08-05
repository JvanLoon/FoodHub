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

        // Whether an account is enabled is carried by EmailConfirmed alone (see
        // ToggleUserEndpoint). Lockout used to double as that flag, which is precisely why it
        // never got round to doing its own job.
        if (!user.EmailConfirmed)
        {
            await Send.StringAsync(ResponseMessages.Account.EmailNotConfirmed, 401, cancellation: ct);
            return;
        }

        // lockoutOnFailure: true is the entire brute-force protection. Identity counts the
        // failure, locks the account for Lockout.DefaultLockoutTimeSpan once
        // Lockout.MaxFailedAccessAttempts is reached, refuses while a lockout is live, and
        // resets the count on success.
        //
        // Counting failures by hand — AccessFailedAsync after the fact — does not work: it
        // writes LockoutEnd, but every read of it goes through UserManager.IsLockedOutAsync,
        // which returns false outright when LockoutEnabled is off. The count went up and
        // nothing ever acted on it.
        var result = await signInManager.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            await Send.StringAsync(ResponseMessages.Account.UserLockedOut, 401, cancellation: ct);
            return;
        }

        if (!result.Succeeded)
        {
            await Send.StringAsync(ResponseMessages.Account.InvalidPassword, 401, cancellation: ct);
            return;
        }

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), new Claim(JwtRegisteredClaimNames.Email, user.Email!),

            // What makes the token revocable. Every request compares this against the account's
            // current stamp, so anything that rotates it — disabling, a role change, a password
            // reset — invalidates this token at once instead of in twelve hours.
            // See SecurityStampCheck.
            new Claim(userManager.Options.ClaimsIdentity.SecurityStampClaimType,
                await userManager.GetSecurityStampAsync(user)),
        ];

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles) { claims.Add(new Claim(ClaimTypes.Role, role)); }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"], 
            claims: claims, 
            expires: DateTime.UtcNow.AddHours(12), 
            signingCredentials: creds);

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