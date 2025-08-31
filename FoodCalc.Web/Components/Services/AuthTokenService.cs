using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace FoodCalc.Web.Components.Services
{
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

        public async Task SetTokenAsync(string token)
        {
            // Set cookie with Secure and SameSite=Strict
            await js.InvokeVoidAsync("eval", $"document.cookie = 'authToken={token}; path=/; secure; samesite=strict';");
        }
    }
}
