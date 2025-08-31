using Blazored.LocalStorage;

using ErrorOr;

using Microsoft.JSInterop;

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
namespace FoodCalc.Web.Components.Services.Auth;

public class AuthTokenService(ILocalStorageService localStorage)
{
	private readonly string _tokenName = "Authorization";
    public async Task<string?> GetTokenAsync()
    {
        return await localStorage.GetItemAsync<string>(_tokenName);
    }

    /// <summary>
    /// If empty string cookie is cleared
    /// </summary>
    public async Task SetTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            await RemoveTokenAsync();
        }
        else
        {
            await localStorage.SetItemAsync(_tokenName, token);
        }
    }

    public async Task RemoveTokenAsync()
    {
		await localStorage.RemoveItemAsync(_tokenName);
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
        var roles = jwt.Claims.Where(c => c.Type.Contains("role")).Select(c => c.Value).ToList();
        
        return roles;
    }
}
