using FoodCalc.Web.Services.Auth;
using FoodHub.DTOs;

namespace FoodCalc.Web.Services.User;

public class UserService(AuthenticatedHttpClientService httpClient)
{
	// TODO(blacklist): these are stubs — they ignore recipeId and just refetch users.
	// Wire them to real add/remove blacklist endpoints when the blacklist feature is built.
	public async Task<List<UserDto>> AddRecipeToBlackList(Guid recipeId)
	{
		var result = await httpClient.GetAsync<PagedResultDto<UserDto>>(ApiRoutes.Admin.Users);
		return result.Success ? result.Data!.Items.ToList() : [];
	}

	public async Task<List<UserDto>> RemoveRecipeToBlackList(Guid recipeId)
	{
		var result = await httpClient.GetAsync<PagedResultDto<UserDto>>(ApiRoutes.Admin.Users);
		return result.Success ? result.Data!.Items.ToList() : [];
	}
}