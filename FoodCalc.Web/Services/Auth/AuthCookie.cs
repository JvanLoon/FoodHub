using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace FoodCalc.Web.Services.Auth;

/// <summary>
/// Turns the API's JWT into this app's auth cookie, and back.
///
/// The browser never talks to the API — <see cref="AuthenticatedHttpClientService"/> runs in this
/// server process against an internal address, and the api container publishes no ports. The token
/// used to make a pointless round trip to local storage purely so the server could read it back
/// through JS interop, where any injected script could read it too. It now stays here, inside a
/// cookie the browser can send but not read.
/// </summary>
public static class AuthCookie
{
    public const string Scheme = CookieAuthenticationDefaults.AuthenticationScheme;

    public const string CookieName = "FoodHub.Auth";

    /// <summary>
    /// A second cookie holding nothing but the fact that a session exists — no identity, no
    /// token, no secret. It is the answer to the one thing SameSite=Strict costs.
    ///
    /// Arriving from anywhere else — a link in a mail, a chat, a search result — is a cross-site
    /// navigation, so the Strict cookie is not sent and the request looks signed out. This one is
    /// Lax, so it *is* sent on exactly that kind of navigation, and its presence next to an
    /// unauthenticated request means "you are signed in, this hop just could not prove it".
    /// The page then reloads itself once (see App.razor), which is same-site, and the real cookie
    /// arrives with the URL untouched.
    ///
    /// Not a server redirect: a redirect inherits the initiator of the navigation that caused it,
    /// so the second request is still cross-site and still gets nothing. It has to be the loaded
    /// document asking.
    /// </summary>
    public const string HintCookieName = "FoodHub.Session";

    /// <summary>Set by the middleware when this request should reload itself once. Read by App.razor.</summary>
    public const string RetryFlag = "FoodHub.RetryForCookie";

    public const string LogoutPath = "/auth/logout";

    /// <summary>
    /// Where the JWT itself lives: a private claim on the principal, and so inside the encrypted
    /// ticket.
    ///
    /// The alternative is <see cref="AuthenticationProperties"/> / <c>StoreTokens</c>, which is
    /// the usual home for a token — but reading it back needs an <see cref="HttpContext"/>, and a
    /// circuit has none. A claim rides to the circuit on the ClaimsPrincipal itself, which is
    /// exactly what <c>AuthenticationStateProvider</c> hands out. Both end up in the same
    /// Data-Protection-encrypted cookie, so nothing is given away by preferring the one that can
    /// actually be read where it is needed.
    /// </summary>
    public const string TokenClaim = "foodhub:token";

    /// <summary>
    /// Signs the browser in from a freshly issued JWT. Only callable during a real HTTP request —
    /// see the class remarks on why that is not a limitation here.
    /// </summary>
    public static async Task SignInAsync(HttpContext http, string jwt)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

        var claims = new List<Claim> { new(TokenClaim, jwt) };

        // The raw claim from the API is "sub"; the inbound mapping that would rename it runs over
        // there, not here. Restated under the standard types so IsInRole and Identity.Name work.
        var subject = Find(token, JwtRegisteredClaimNames.Sub) ?? Find(token, ClaimTypes.NameIdentifier);
        if (subject is not null)
            claims.Add(new Claim(ClaimTypes.NameIdentifier, subject));

        var email = Find(token, JwtRegisteredClaimNames.Email) ?? Find(token, ClaimTypes.Email);
        if (email is not null)
        {
            claims.Add(new Claim(ClaimTypes.Email, email));
            claims.Add(new Claim(ClaimTypes.Name, email));
        }

        claims.AddRange(token.Claims.Where(c => c.Type is ClaimTypes.Role or "role" or "roles")
            .Select(c => new Claim(ClaimTypes.Role, c.Value)));

        var identity = new ClaimsIdentity(claims, Scheme, ClaimTypes.Name, ClaimTypes.Role);

        // Tied to the JWT's own expiry. A cookie outliving the token would leave the UI looking
        // signed in while every API call behind it came back 401.
        var properties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = token.ValidTo
        };

        await http.SignInAsync(Scheme, new ClaimsPrincipal(identity), properties);

        WriteHint(http, token.ValidTo);
    }

    /// <summary>Signs out and clears the hint alongside, so nothing is left claiming a session.</summary>
    public static async Task SignOutAsync(HttpContext http)
    {
        await http.SignOutAsync(Scheme);
        http.Response.Cookies.Delete(HintCookieName, HintOptions(expires: null));
    }

    /// <summary>
    /// Keeps the hint in step with the real cookie, and turns "hint but no session" into the
    /// one-shot reload that recovers it.
    ///
    /// Deleting the hint before asking for that reload is what stops it looping. If the reload
    /// comes back authenticated the branch above puts the hint straight back; if it does not —
    /// the session really is over, or the browser is refusing the Strict cookie outright — there
    /// is no longer anything to trigger a second attempt, and the request falls through to the
    /// login page as it should.
    /// </summary>
    public static IApplicationBuilder UseSessionHint(this IApplicationBuilder app) => app.Use(async (http, next) =>
    {
        var signedIn = http.User.Identity?.IsAuthenticated == true;
        var hasHint = http.Request.Cookies.ContainsKey(HintCookieName);

        if (signedIn)
        {
            if (!hasHint)
                WriteHint(http, http.Features.Get<IAuthenticateResultFeature>()
                    ?.AuthenticateResult?.Properties?.ExpiresUtc);
        }
        else if (hasHint && IsDocumentRequest(http.Request))
        {
            http.Response.Cookies.Delete(HintCookieName, HintOptions(expires: null));
            http.Items[RetryFlag] = true;
        }

        await next();
    });

    /// <summary>
    /// A top-level page load, as opposed to the static assets, the SignalR circuit and the health
    /// probe. Only a document can reload itself, and only a document is worth spending a reload on.
    /// </summary>
    private static bool IsDocumentRequest(HttpRequest request) =>
        HttpMethods.IsGet(request.Method) && request.Headers.Accept.ToString()
            .Contains("text/html", StringComparison.OrdinalIgnoreCase);

    private static void WriteHint(HttpContext http, DateTimeOffset? expires) =>
        http.Response.Cookies.Append(HintCookieName, "1", HintOptions(expires));

    private static CookieOptions HintOptions(DateTimeOffset? expires) => new()
    {
        // HttpOnly even though it holds nothing worth reading: there is no reason for script to
        // touch it, and the reload is triggered by the server rendering a marker, not by JS
        // sniffing for this.
        HttpOnly = true,
        Secure = true,

        // The whole point of this cookie. Lax is what gets it through the cross-site navigation
        // the Strict one cannot survive.
        SameSite = SameSiteMode.Lax,
        Path = "/",
        Expires = expires
    };

    private static string? Find(JwtSecurityToken token, string type) =>
        token.Claims.FirstOrDefault(c => c.Type == type)
            ?.Value;
}
