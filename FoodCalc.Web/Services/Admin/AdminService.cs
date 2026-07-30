using FoodCalc.Web.Services.Auth;
using FoodHub.DTOs;

namespace FoodCalc.Web.Services.Admin;

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

        return ApiResult<List<UserDto>>.Ok([..paged.Data!.Items], paged.StatusCode);
    }

    // Posted as a body rather than a query string, so the shape is checked by the compiler
    // against the API's request type — and so an address containing '+' survives the trip.
    public Task<ApiResult> ToggleUserAsync(string email, bool enable = true) => httpClient.PostAsync(
        ApiRoutes.Authentication.ToggleUser, new ToggleUserRequest
        {
            Email = email,
            Enable = enable
        });

    /// <summary>
    /// Development diagnostics: asks the API to fail with <paramref name="count"/> errors so the
    /// multi-error path can be exercised end to end. On a 2xx status the API returns a short text
    /// body naming the status (e.g. "200 => OK"), surfaced as the success payload.
    /// </summary>
    public Task<ApiResult<string>> TriggerErrorTestAsync(int count, int statusCode) =>
        httpClient.GetAsync<string>($"{ApiRoutes.Dev.ErrorTest}?count={count}&statusCode={statusCode}");

    public async Task<ApiResult<List<string>>> GetAllRolesAsync()
    {
        // Roles feed the role-picker, which needs every role, so fetch all in one page.
        var paged = await httpClient.GetAsync<PagedResultDto<string>>(
            $"{ApiRoutes.Admin.AllRoles}?page=1&pageSize={int.MaxValue}");
        if (!paged.Success)
            return ApiResult<List<string>>.Fail(paged.Errors, paged.StatusCode);

        return ApiResult<List<string>>.Ok([..paged.Data!.Items], paged.StatusCode);
    }

    // GET and DELETE have no body, so these stay query strings — but the values still have to
    // be escaped, or a '+' in an address arrives as a space and the lookup returns 404.
    public Task<ApiResult<List<string>>> GetUserRolesAsync(string email) =>
        httpClient.GetAsync<List<string>>($"{ApiRoutes.Admin.UserRoles}?email={Uri.EscapeDataString(email)}");

    public Task<ApiResult> UpdateUserRolesAsync(string email, string newRole) => httpClient.PostAsync(
        ApiRoutes.Admin.UserRoles, new ModifyUserRoleRequest
        {
            Email = email,
            Role = newRole
        });

    public Task<ApiResult> RemoveUserRoleAsync(string email, string role) => httpClient.DeleteAsync(
        $"{ApiRoutes.Admin.UserRoles}?email={Uri.EscapeDataString(email)}&role={Uri.EscapeDataString(role)}");

    /// <summary>Deletes the account outright. The API refuses if it is the caller's own.</summary>
    public Task<ApiResult> DeleteUserAsync(string email) =>
        httpClient.DeleteAsync($"{ApiRoutes.Admin.User}?email={Uri.EscapeDataString(email)}");
}