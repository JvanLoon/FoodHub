using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using FoodCalc.Web.Constants;

namespace FoodCalc.Web.Services.Auth;

public class AuthTokenService(ILocalStorageService localStorage)
{
    private readonly string _tokenName = WebConstants.Storage.AuthToken;

    public async Task<string?> GetTokenAsync()
    {
        return await localStorage.GetItemAsync<string>(_tokenName);
    }

    public async Task SetTokenAsync(string token)
    {
        if (!string.IsNullOrEmpty(token)) { await localStorage.SetItemAsync(_tokenName, token); }
    }

    public async Task RemoveTokenAsync()
    {
        await localStorage.RemoveItemAsync(_tokenName);
    }

    /// <summary>
    /// The stored token's claims, or null when there is no token or it cannot be parsed.
    ///
    /// Local storage is the user's to edit, and a value left behind by an older build may not be
    /// a JWT at all. To every caller here the two cases mean the same thing — there is nothing
    /// usable — so a garbled token is reported as absent rather than thrown out of a lifecycle
    /// method, where it would surface as a crashed component instead of a login page.
    /// </summary>
    private async Task<JwtSecurityToken?> ReadTokenAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        try { return new JwtSecurityTokenHandler().ReadJwtToken(token); }
        catch (ArgumentException) { return null; }
    }

    public async Task<string?> GetEmailAsync()
    {
        var jwt = await ReadTokenAsync();

        return jwt?.Claims.FirstOrDefault(c => c.Type == "email")
            ?.Value;
    }

    /// <summary>
    /// The logged-in account's IdentityUser id, read from the token's "sub" claim (see
    /// LoginEndpoint). Used to tell the user's own content from everyone else's — the server
    /// enforces that independently, this is only so the UI can show the right controls.
    /// </summary>
    public async Task<string?> GetUserIdAsync()
    {
        var jwt = await ReadTokenAsync();
        if (jwt is null)
            return null;

        // The raw claim is "sub"; the inbound-claim mapping that would rename it to
        // nameidentifier runs on the API, not here, so check both.
        return jwt.Claims.FirstOrDefault(c => c.Type == "sub")
            ?.Value ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value;
    }

    /// <summary>
    /// True unless there is a readable token with an expiry still in the future. Every other
    /// case — nothing stored, not a JWT, no <c>exp</c>, an <c>exp</c> that is not a number —
    /// counts as expired, because none of them can be used to call the API.
    /// </summary>
    public async Task<bool> IsTokenExpiredAsync()
    {
        var jwt = await ReadTokenAsync();

        var exp = jwt?.Claims.FirstOrDefault(c => c.Type == "exp")
            ?.Value;

        if (!long.TryParse(exp, out var seconds))
            return true;

        return DateTimeOffset.FromUnixTimeSeconds(seconds) < DateTimeOffset.UtcNow;
    }

    public async Task<List<string>> GetRolesAsync()
    {
        var jwt = await ReadTokenAsync();
        if (jwt is null)
            return [];

        return jwt.Claims.Where(c => c.Type.Contains("role"))
            .Select(c => c.Value)
            .ToList();
    }
}