using FoodHub.DTOs;

using Microsoft.AspNetCore.Components;

using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FoodCalc.Web.Components.Services;
public class LoginService
{
    private readonly HttpClient _httpClient;

    public LoginService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<HttpResponseMessage> LoginAsync(LoginDto user)
    {
		return await _httpClient.PostAsJsonAsync("api/authentication/login", user); ;

	}

    public async Task<bool> RegisterAsync(RegisterDto user)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/authentication/register", user);
        return response.IsSuccessStatusCode;
    }
}
