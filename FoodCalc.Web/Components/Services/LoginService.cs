using Blazored.LocalStorage;

using FoodCalc.Web.Components.Services.Auth;

using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;
public class LoginService//(AuthenticatedHttpClientService httpClient)
{
    private readonly HttpClient _httpClient;
	private readonly AuthenticatedHttpClientService _authHttpClient;

	public LoginService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
		_authHttpClient = new AuthenticatedHttpClientService(_httpClient, localStorageService);
	}

    public async Task<HttpResponseMessage> LoginAsync(LoginDto user)
    {
		return await _httpClient.PostAsJsonAsync("api/authentication/login", user); ;
	}

    public async Task<(bool Success, string ErrorMessage)> RegisterAsync(RegisterDto user)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/authentication/register", user);

		if (response.IsSuccessStatusCode)
		{
			return (true, null);
		}
		else
		{
			var error = await response.Content.ReadAsStringAsync();
			return (false, error);
		}
	}

	public async Task<HttpResponseMessage> ResetPassword(ResetPasswordDto resetPasswordRequest)
	{
		return await _authHttpClient.PostAsJsonAsync("api/authentication/resetpassword", resetPasswordRequest);
	}
}
