using Microsoft.AspNetCore.Components.Authorization;

namespace FoodCalc.Web.Services.Auth;

/// <summary>
/// The JWT for the current user, for the one thing that still needs the token itself: the
/// Authorization header on calls to the API.
///
/// Reads it off the ClaimsPrincipal rather than the cookie, which is what makes it work in both
/// worlds. During the HTTP render the principal comes from the cookie; inside the circuit, where
/// there is no <c>HttpContext</c> to read a cookie from, the same principal is what
/// <see cref="AuthenticationStateProvider"/> already holds. Nothing has to be captured and handed
/// across — and it could not be, since the HTTP render and the circuit are separate DI scopes.
/// </summary>
public sealed class TokenProvider(AuthenticationStateProvider authenticationStateProvider)
{
    public async Task<string?> GetTokenAsync()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();

        return state.User.FindFirst(AuthCookie.TokenClaim)
            ?.Value;
    }
}
