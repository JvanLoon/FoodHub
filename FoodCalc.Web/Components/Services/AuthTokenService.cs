using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.JSInterop;
using System.Threading.Tasks;
namespace FoodCalc.Web.Components.Services;

public class AuthTokenService(IJSRuntime js)
{
    public async Task<string?> GetTokenAsync()
    {
        var cookie = await js.InvokeAsync<string>("eval", "document.cookie");
        var token = cookie?.Split(';')
            .Select(c => c.Trim())
            .FirstOrDefault(c => c.StartsWith("authToken="));
        return token != null ? token.Substring("authToken=".Length) : null;
    }

    /// <summary>
    /// If empty string cookie is cleared
    /// </summary>
    public async Task SetTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            //delete cookie
            await RemoveTokenAsync();
        }
        else
        {
            // Set cookie with Secure and SameSite=Strict
            await js.InvokeVoidAsync("eval", $"document.cookie = 'authToken={token}; path=/; secure; samesite=strict';");
        }
    }

    public async Task RemoveTokenAsync()
    {
        await js.InvokeVoidAsync("eval", $"document.cookie = 'authToken=; path=/; secure; samesite=strict';");
    }

    public async Task<string?> GetEmailAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == "email");

        return emailClaim?.Value;
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return true;

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var exp = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

        if (exp == null)
            return true;

        var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));
        return expDate < DateTimeOffset.UtcNow;
    }
    
    public async Task<List<string>> GetRolesAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return new List<string>();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var roles = jwt.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
        
        return roles;
    }
}
