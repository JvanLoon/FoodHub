using Azure;

using FoodHub.DTOs;

using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FoodCalc.Web.Components.Services;
public class AdminService
{
    private readonly HttpClient _httpClient;

    public AdminService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
		var response = await _httpClient.GetAsync("api/Admin/users");
		if (!response.IsSuccessStatusCode)
		{
			// Optionally log or handle the error
			var errorContent = await response.Content.ReadAsStringAsync();
			// Handle 401, 403, 500, etc.
			return [];
		}
		return await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? [];
	}
}
