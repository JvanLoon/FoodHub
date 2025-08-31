using Microsoft.JSInterop;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace FoodCalc.Web.Components.Services
{
    public class AuthTokenHandler(IJSRuntime js) : DelegatingHandler
    {
        private readonly string[] _excludedPaths = new[]
        {
            "/api/authentication/login",
            "/api/authentication/register"
            // Add more paths to exclude if needed
        };


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!_excludedPaths.Any(path => request.RequestUri?.AbsolutePath.StartsWith(path) == true))
            {
                var cookie = await js.InvokeAsync<string>("eval", "document.cookie");
                var token = cookie?.Split(';')
                    .Select(c => c.Trim())
                    .FirstOrDefault(c => c.StartsWith("authToken="));
                if (token != null)
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Substring("authToken=".Length));
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
