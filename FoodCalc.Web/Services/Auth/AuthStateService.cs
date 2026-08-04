namespace FoodCalc.Web.Services.Auth;

public class AuthStateService(AuthTokenService authTokenService, PresenceService presenceService)
{
    public event Func<Task>? OnAuthStateChanged;

    public async Task<bool> IsLoggedInAsync() => !await authTokenService.IsTokenExpiredAsync();

    public async Task<string?> GetEmailAsync() => await authTokenService.GetEmailAsync();

    /// <summary>The logged-in account's IdentityUser id, for "is this mine?" checks in the UI.</summary>
    public async Task<string?> GetUserIdAsync() => await authTokenService.GetUserIdAsync();

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

    public async Task<List<string>> GetRolesAsync() => await authTokenService.GetRolesAsync();

    public async Task<bool> IsAdminAsync()
    {
        var roles = await authTokenService.GetRolesAsync();
        return roles.Contains("Admin");
    }

    /// <summary>True if the logged-in user holds at least one of the given roles.</summary>
    public async Task<bool> IsInAnyRoleAsync(params string[] roles)
    {
        var mine = await authTokenService.GetRolesAsync();
        return mine.Any(r => roles.Contains(r, StringComparer.OrdinalIgnoreCase));
    }

    public async Task SignInAsync(string token)
    {
        await authTokenService.SetTokenAsync(token);
        presenceService.Start();
        await NotifyAuthStateChangedAsync();
    }

    /// <summary>
    /// Throws away a token that could not be used anyway — expired, or not a JWT at all.
    ///
    /// Not SignOutAsync: that pings the API to mark the account offline, and the ping is
    /// authenticated with the very token being discarded, so it could only ever fail. There is
    /// also nothing to mark offline — presence lapsed on its own long before the token did.
    /// </summary>
    public async Task DiscardTokenAsync()
    {
        await authTokenService.RemoveTokenAsync();
        await NotifyAuthStateChangedAsync();
    }

    public async Task SignOutAsync()
    {
        // Before the token goes: the ping that marks the account offline is itself authenticated,
        // so it has to ride out on the credentials being discarded.
        await presenceService.SignalOfflineAsync();
        await authTokenService.RemoveTokenAsync();
        await NotifyAuthStateChangedAsync();
    }

    public async Task NotifyAuthStateChangedAsync()
    {
        if (OnAuthStateChanged == null) return;
        foreach (var handler in OnAuthStateChanged.GetInvocationList()
            .Cast<Func<Task>>())
        {
            try { await handler(); }
            catch {}
        }
    }
}