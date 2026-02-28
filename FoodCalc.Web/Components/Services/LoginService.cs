using FoodCalc.Web.Components.Services.Auth;

using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;

public class LoginService(IHttpClientFactory httpClientFactory, AuthenticatedHttpClientService authHttpClient)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    public async Task<HttpResponseMessage> LoginAsync(LoginDto user)
    {
        return await _httpClient.PostAsJsonAsync("api/authentication/login", user);
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterDto user)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/authentication/register", user);

        if (response.IsSuccessStatusCode)
        {
			return (true, null);
		}

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }

    public async Task<HttpResponseMessage> ResetPassword(ResetPasswordDto resetPasswordRequest)
    {
        return await authHttpClient.PostAsJsonAsync("api/authentication/resetpassword", resetPasswordRequest);
    }
}
