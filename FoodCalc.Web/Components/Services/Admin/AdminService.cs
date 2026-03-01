using FoodCalc.Web.Components.Services.Auth;

using FoodHub.DTOs;

using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FoodCalc.Web.Components.Services.Admin;
public class AdminService(AuthenticatedHttpClientService httpClient)
{
	public async Task<PagedResultDto<UserDto>> GetPagedUsersAsync(int page, int pageSize, string? search = null)
	{
		var url = $"api/Admin/users?page={page}&pageSize={pageSize}";
		if (!string.IsNullOrWhiteSpace(search))
			url += $"&search={Uri.EscapeDataString(search)}";

		var response = await httpClient.GetAsync(url);
		if (!response.IsSuccessStatusCode)
			return new();
		return await response.Content.ReadFromJsonAsync<PagedResultDto<UserDto>>() ?? new();
	}

	public async Task<List<UserDto>> GetUsersAsync()
	{
		var paged = await GetPagedUsersAsync(1, int.MaxValue);
		return [..paged.Items];
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
		var response = await httpClient.PostAsync($"api/Admin/userroles?email={email}&role={newRole}");
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> RemoveUserRoleAsync(string email, string role)
	{
		var response = await httpClient.DeleteAsync($"api/Admin/userroles?email={email}&role={role}");
		return response.IsSuccessStatusCode;
	}
}
