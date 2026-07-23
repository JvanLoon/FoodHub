using FoodCalc.Web.Services.Auth;

using FoodHub.DTOs;

namespace FoodCalc.Web.Services;

public class LoginService(AuthenticatedHttpClientService httpClient)
{
	public Task<ApiResult<AuthResponseDto>> LoginAsync(LoginDto user) =>
		httpClient.PostAsync<LoginDto, AuthResponseDto>(ApiRoutes.Authentication.Login, user);

	public Task<ApiResult> RegisterAsync(RegisterDto user) =>
		httpClient.PostAsync(ApiRoutes.Authentication.Register, user);

	public Task<ApiResult> ResetPassword(ResetPasswordDto resetPasswordRequest) =>
		httpClient.PostAsync(ApiRoutes.Authentication.ResetPassword, resetPasswordRequest);
}
