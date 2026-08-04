using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace FoodCalc.Api.Common;

/// <summary>
/// Turns the JWT from a twelve-hour promise into something that can be withdrawn.
///
/// The token is self-contained: signature and expiry are all the handler checks, and neither
/// depends on the account still existing, still being enabled, or still holding the roles baked
/// into it. Disabling an account, deleting it, or taking away its Admin role therefore did
/// nothing at all until its token ran out on its own.
///
/// Identity already keeps the right primitive for this. Every user carries a SecurityStamp that
/// changes whenever their credentials or standing do; putting it in the token and comparing on
/// each request means anything that rotates the stamp cuts off every token issued before it,
/// immediately. See <see cref="UserManager{TUser}.UpdateSecurityStampAsync"/> and the callers of
/// it in the admin endpoints.
/// </summary>
public static class SecurityStampCheck
{
    /// <summary>
    /// Rejects a token whose stamp no longer matches the account's. Wired to
    /// <see cref="JwtBearerEvents.OnTokenValidated"/>, so it runs after the signature and expiry
    /// have already passed and only ever narrows what is accepted.
    /// </summary>
    public static async Task OnTokenValidatedAsync(TokenValidatedContext context)
    {
        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();

        var userId = context.Principal?.GetUserId();

        // Tokens minted before this check existed carry no stamp. Failing them is the point:
        // they were issued under rules that could not revoke them, so everyone signs in once
        // more and the old ones stop working.
        var stamp = context.Principal?.FindFirstValue(userManager.Options.ClaimsIdentity.SecurityStampClaimType);

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(stamp))
        {
            context.Fail("Token carries no security stamp.");
            return;
        }

        // Deleting an account revokes it here, with nothing extra to remember: there is no user
        // left to match a stamp against.
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            context.Fail("Account no longer exists.");
            return;
        }

        var current = await userManager.GetSecurityStampAsync(user);
        if (!string.Equals(current, stamp, StringComparison.Ordinal))
            context.Fail("Token was issued before the account last changed.");
    }
}
