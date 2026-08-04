using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace FoodCalc.Api.Endpoints.Authentication;

/// <summary>
/// POST api/authentication/toggleUser — Admin. Takes <see cref="ToggleUserRequest"/> as a
/// JSON body; the type lives in FoodHub.DTOs so the Web client posts the same shape.
/// </summary>
public class ToggleUserEndpoint(UserManager<IdentityUser> userManager) : Endpoint<ToggleUserRequest>
{
    public override void Configure()
    {
        Post(ApiRoutes.Authentication.ToggleUser);
        Policies("Admin");
    }

    public override async Task HandleAsync(ToggleUserRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user == null)
        {
            await Send.StringAsync(ResponseMessages.Account.UserNotFound, 404, cancellation: ct);
            return;
        }

        user.EmailConfirmed = req.Enable;

        // Always on, in both branches. LockoutEnabled means "may this account be locked out",
        // not "is it disabled" — and clearing it here is what left every working account with no
        // brute-force protection at all, because UserManager.IsLockedOutAsync short-circuits to
        // false when it is off. Asserted rather than assumed, so any account that predates the
        // fix heals the next time it is toggled.
        user.LockoutEnabled = true;

        if (req.Enable)
        {
            // Clear a live lockout so a re-enabled account is not still serving out a ban, and
            // reset the count that produced it.
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
        }
        else
        {
            // Beside EmailConfirmed rather than instead of it: now that lockout is genuinely
            // enforced, this also stops any sign-in path that goes through SignInManager without
            // repeating the EmailConfirmed check.
            user.LockoutEnd = DateTimeOffset.MaxValue;
        }

        if (!await userManager.IsInRoleAsync(user, "User")) { await userManager.AddToRoleAsync(user, "User"); }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            await Send.ResultAsync(TypedResults.BadRequest(result.Errors));
            return;
        }

        // What actually kicks a disabled account out. Their token is valid for twelve hours and
        // carries its own roles, so without this "disable" only stopped them signing in again —
        // an open session carried on untouched. Rotating the stamp invalidates every token
        // already issued to them. See SecurityStampCheck.
        //
        // Done on enable as well: that path grants the User role, and a token minted before the
        // grant would not carry it.
        await userManager.UpdateSecurityStampAsync(user);

        await Send.OkAsync(cancellation: ct);
    }
}