using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodCalc.Api.Extensions;

/// <summary>
/// Runtime replacement for the old <c>IdentitySeed</c> HasData block.
///
/// The roles are part of the application's contract (the authorization policies in
/// Program.cs name them), so they are always ensured — creating a role stores no
/// secret and is safe to repeat on every boot.
///
/// The first admin account is different: it needs a password, and a password baked
/// into a migration ends up in git. So it is created only when BOTH of these hold:
///   • Bootstrap:AdminEmail and Bootstrap:AdminPassword are supplied (env vars), and
///   • the database contains no users at all.
/// That makes it a genuine one-shot: after the first successful boot the account
/// exists, the "no users" check fails, and the variables can be removed from the
/// environment. A deployment that supplies neither variable seeds nothing.
/// </summary>
public static class IdentityBootstrapExtensions
{
    public static readonly string[] ApplicationRoles = ["Admin", "Moderator", "User"];

    public static async Task BootstrapIdentityAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("IdentityBootstrap");

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in ApplicationRoles)
        {
            if (await roleManager.RoleExistsAsync(role))
                continue;

            var created = await roleManager.CreateAsync(new IdentityRole(role));
            if (created.Succeeded)
                logger.LogInformation("Created missing role {Role}.", role);
            else
                throw new InvalidOperationException($"Could not create role '{role}': {Describe(created)}");
        }

        var email = app.Configuration["Bootstrap:AdminEmail"];
        var password = app.Configuration["Bootstrap:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogInformation("No bootstrap admin configured; skipping account creation.");
            return;
        }

        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        if (await userManager.Users.AnyAsync())
        {
            logger.LogInformation(
                "Bootstrap admin skipped: the database already has users. Remove Bootstrap:AdminEmail " +
                "and Bootstrap:AdminPassword from the environment.");
            return;
        }

        var admin = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true // Enabled/disabled is gated on EmailConfirmed, see ToggleUserEndpoint.
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Could not create the bootstrap admin account: {Describe(result)}");

        // Lockout is deliberately left on, which CreateAsync does for us while
        // Lockout.AllowedForNewUsers is set. This account used to have it switched off again
        // straight afterwards, to protect the one login that can reach the admin UI from being
        // locked out by five bad guesses — including your own.
        //
        // That trade was the wrong way round. The cost of leaving it on is a ten-minute wait
        // (Lockout.DefaultLockoutTimeSpan) after five failures. The cost of turning it off was
        // an admin account that could be guessed at forever, and it is the single most valuable
        // account in the system.
        var roled = await userManager.AddToRolesAsync(admin, ApplicationRoles);
        if (!roled.Succeeded)
            throw new InvalidOperationException(
                $"Created the bootstrap admin but could not assign its roles: {Describe(roled)}");

        logger.LogWarning(
            "Created bootstrap admin {Email}. Sign in, change the password, then clear the " +
            "Bootstrap:* environment variables.", email);
    }

    private static string Describe(IdentityResult result) =>
        string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
}