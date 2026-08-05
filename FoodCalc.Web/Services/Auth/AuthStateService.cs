using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace FoodCalc.Web.Services.Auth;

/// <summary>
/// The UI's view of who is signed in, read off the ClaimsPrincipal behind the auth cookie.
///
/// Everything here only decides what is worth rendering. The API re-checks every request against
/// the JWT independently, so a wrong answer here shows the wrong buttons — it does not grant
/// anything.
///
/// There is no SignIn here any more: signing in writes a cookie, which only a real HTTP request
/// can do (see <see cref="AuthCookie"/>). Login is a form post to a static-rendered page, and
/// signing out is a navigation to <see cref="AuthCookie.LogoutPath"/>. What is left of sign-out
/// on this side is the part that has to happen first, while the JWT is still usable: telling the
/// API the user has gone.
/// </summary>
public class AuthStateService(AuthenticationStateProvider authenticationStateProvider, PresenceService presenceService)
{
    private async Task<ClaimsPrincipal> UserAsync() =>
        (await authenticationStateProvider.GetAuthenticationStateAsync()).User;

    public async Task<bool> IsLoggedInAsync() => (await UserAsync()).Identity?.IsAuthenticated == true;

    public async Task<string?> GetEmailAsync() => (await UserAsync()).FindFirst(ClaimTypes.Email)
        ?.Value;

    /// <summary>The logged-in account's IdentityUser id, for "is this mine?" checks in the UI.</summary>
    public async Task<string?> GetUserIdAsync() => (await UserAsync()).FindFirst(ClaimTypes.NameIdentifier)
        ?.Value;

    /// <summary>
    /// True for staff (Admin or Moderator), who may edit any recipe or ingredient. Mirrors the
    /// server's ActingUser rule; the API re-checks on every write, so this only gates the UI.
    /// </summary>
    public async Task<bool> CanEditAnyContentAsync() => await IsInAnyRoleAsync("Admin", "Moderator");

    /// <summary>
    /// True if the UI should offer edit controls for content authored by
    /// <paramref name="ownerUserId"/>: staff for anything, or the author for their own. The API
    /// re-checks on every write, so this only decides what is worth rendering.
    /// </summary>
    public async Task<bool> CanEditContentAsync(string ownerUserId)
    {
        if (await CanEditAnyContentAsync())
            return true;

        var userId = await GetUserIdAsync();
        return !string.IsNullOrEmpty(userId) && userId == ownerUserId;
    }

    public async Task<List<string>> GetRolesAsync() => (await UserAsync()).FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList();

    public async Task<bool> IsAdminAsync() => (await UserAsync()).IsInRole("Admin");

    /// <summary>True if the logged-in user holds at least one of the given roles.</summary>
    public async Task<bool> IsInAnyRoleAsync(params string[] roles)
    {
        var user = await UserAsync();
        return roles.Any(user.IsInRole);
    }

    /// <summary>
    /// Marks the account offline and stops the heartbeat, before the caller navigates to
    /// <see cref="AuthCookie.LogoutPath"/> to drop the cookie.
    ///
    /// Order matters and cannot be reversed: the ping that marks the account offline is itself
    /// authenticated with the JWT inside the cookie, so it has to go out while that cookie is
    /// still there.
    /// </summary>
    public async Task SignalSignOutAsync() => await presenceService.SignalOfflineAsync();
}
