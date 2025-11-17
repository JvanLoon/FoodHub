using FoodCalc.Web.Components.Services.Auth;

using FoodHub.DTOs;

using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FoodCalc.Web.Components.Services.Admin;
public class AdminService(AuthenticatedHttpClientService httpClient)
{
    public async Task<List<UserDto>> GetUsersAsync()
    {
        var response = await httpClient.GetAsync("api/Admin/users");
        if (!response.IsSuccessStatusCode)
        {
            // Optionally log or handle the error
            var errorContent = await response.Content.ReadAsStringAsync();
            // Handle 401, 403, 500, etc.
            return [];
        }
        return await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? [];
    }

    public async Task<bool> ToggleUserAsync(string email, bool enable = true)
    {
        // Specify the type argument explicitly to resolve CS0411
        var response = await httpClient.PostAsync($"api/authentication/toggleUser?email={email}&enable={enable}", null);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }
        return true;
    }

	public async Task<List<string>> GetAllRolesAsync()
	{
		var response = await httpClient.GetAsync("api/Admin/allroles");
		if (!response.IsSuccessStatusCode)
			return [];

		var roles = await response.Content.ReadAsStringAsync();
		return roles.Split(",").ToList();
	}

	public async Task<List<string>> GetUserRolesAsync(string email)
	{
		var response = await httpClient.GetAsync($"api/Admin/userroles?email={email}");
		if (!response.IsSuccessStatusCode)
			return [];
		return await response.Content.ReadFromJsonAsync<List<string>>() ?? [];
	}

	public async Task<bool> UpdateUserRolesAsync(string email, string newRole)
	{
		var response = await httpClient.PostAsync($"api/Admin/userroles?email={email}&role=newRole");
		return response.IsSuccessStatusCode;
	}
}
