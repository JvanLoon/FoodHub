using FoodCalc.Web.Components.Services.Auth;

using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;

public class LoginService(AuthenticatedHttpClientService httpClient)
{
    public Task<ApiResult<AuthResponseDto>> LoginAsync(LoginDto user) =>
        httpClient.PostAsync<LoginDto, AuthResponseDto>("api/authentication/login", user);

    public Task<ApiResult> RegisterAsync(RegisterDto user) =>
        httpClient.PostAsync("api/authentication/register", user);

    public Task<ApiResult> ResetPassword(ResetPasswordDto resetPasswordRequest) =>
        httpClient.PostAsync("api/authentication/resetpassword", resetPasswordRequest);
}
