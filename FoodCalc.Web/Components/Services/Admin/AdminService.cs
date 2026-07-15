using FoodCalc.Web.Components.Services.Auth;

using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services.Admin;
public class AdminService(AuthenticatedHttpClientService httpClient)
{
	public Task<ApiResult<PagedResultDto<UserDto>>> GetPagedUsersAsync(int page, int pageSize, string? search = null)
	{
		var url = $"api/admin/users?page={page}&pageSize={pageSize}";
		if (!string.IsNullOrWhiteSpace(search))
			url += $"&search={Uri.EscapeDataString(search)}";

		return httpClient.GetAsync<PagedResultDto<UserDto>>(url);
	}

	public async Task<ApiResult<List<UserDto>>> GetUsersAsync()
	{
		var paged = await GetPagedUsersAsync(1, int.MaxValue);
		if (!paged.Success)
			return ApiResult<List<UserDto>>.Fail(paged.Error!, paged.StatusCode);

		return ApiResult<List<UserDto>>.Ok([.. paged.Data!.Items], paged.StatusCode);
	}

	public Task<ApiResult> ToggleUserAsync(string email, bool enable = true) =>
		httpClient.PostAsync($"api/authentication/toggleUser?email={email}&enable={enable}");

	public async Task<ApiResult<List<string>>> GetAllRolesAsync()
	{
		var result = await httpClient.GetAsync<string>("api/admin/allroles");
		if (!result.Success)
			return ApiResult<List<string>>.Fail(result.Error!, result.StatusCode);

		var roles = (result.Data ?? string.Empty)
			.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.ToList();
		return ApiResult<List<string>>.Ok(roles, result.StatusCode);
	}

	public Task<ApiResult<List<string>>> GetUserRolesAsync(string email) =>
		httpClient.GetAsync<List<string>>($"api/admin/userroles?email={email}");

	public Task<ApiResult> UpdateUserRolesAsync(string email, string newRole) =>
		httpClient.PostAsync($"api/admin/userroles?email={email}&role={newRole}");

	public Task<ApiResult> RemoveUserRoleAsync(string email, string role) =>
		httpClient.DeleteAsync($"api/admin/userroles?email={email}&role={role}");
}
