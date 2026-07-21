using FoodCalc.Web.Components.Services.Auth;

using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services.Admin;

public class AdminService(AuthenticatedHttpClientService httpClient)
{
	public Task<ApiResult<PagedResultDto<UserDto>>> GetPagedUsersAsync(int page, int pageSize, string? search = null)
	{
		var url = $"{ApiRoutes.Admin.Users}?page={page}&pageSize={pageSize}";
		if (!string.IsNullOrWhiteSpace(search))
			url += $"&search={Uri.EscapeDataString(search)}";

		return httpClient.GetAsync<PagedResultDto<UserDto>>(url);
	}

	public async Task<ApiResult<List<UserDto>>> GetUsersAsync()
	{
		var paged = await GetPagedUsersAsync(1, int.MaxValue);
		if (!paged.Success)
			return ApiResult<List<UserDto>>.Fail(paged.Errors, paged.StatusCode);

		return ApiResult<List<UserDto>>.Ok([.. paged.Data!.Items], paged.StatusCode);
	}

	public Task<ApiResult> ToggleUserAsync(string email, bool enable = true) =>
		httpClient.PostAsync($"{ApiRoutes.Authentication.ToggleUser}?email={email}&enable={enable}");

	public async Task<ApiResult<List<string>>> GetAllRolesAsync()
	{
		// Roles feed the role-picker, which needs every role, so fetch all in one page.
		var paged = await httpClient.GetAsync<PagedResultDto<string>>(
			$"{ApiRoutes.Admin.AllRoles}?page=1&pageSize={int.MaxValue}");
		if (!paged.Success)
			return ApiResult<List<string>>.Fail(paged.Errors, paged.StatusCode);

		return ApiResult<List<string>>.Ok([.. paged.Data!.Items], paged.StatusCode);
	}

	public Task<ApiResult<List<string>>> GetUserRolesAsync(string email) =>
		httpClient.GetAsync<List<string>>($"{ApiRoutes.Admin.UserRoles}?email={email}");

	public Task<ApiResult> UpdateUserRolesAsync(string email, string newRole) =>
		httpClient.PostAsync($"{ApiRoutes.Admin.UserRoles}?email={email}&role={newRole}");

	public Task<ApiResult> RemoveUserRoleAsync(string email, string role) =>
		httpClient.DeleteAsync($"{ApiRoutes.Admin.UserRoles}?email={email}&role={role}");
}
