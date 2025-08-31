using Blazored.LocalStorage;

using Microsoft.JSInterop;

using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FoodCalc.Web.Components.Services;
public class AuthTokenHandler(ILocalStorageService localStorage) : DelegatingHandler
{
    private readonly string[] _excludedPaths = new[]
    {
        "/api/authentication/login",
        "/api/authentication/register"
        // Add more paths to exclude if needed
    };


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
		try
		{
			if (!_excludedPaths.Any(path => request.RequestUri?.AbsolutePath.Contains(path) == true))
			{
				var token = await localStorage.GetItemAsync<string>("authToken") ?? string.Empty;
				if (!string.IsNullOrWhiteSpace(token))
				{
					request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				}
			}
		}
		catch (JSException jex)
		{
			// JS interop not available (prerendering), skip adding token
		}
		catch (Exception ex)
		{
			// JS interop not available (prerendering), skip adding token
		}

		return await base.SendAsync(request, cancellationToken);
	}
}
