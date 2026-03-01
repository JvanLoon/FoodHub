using FoodCalc.Web.Components.Services.Auth;
using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;
public class UserService(AuthenticatedHttpClientService httpClient)
{
	public async Task<List<UserDto>> AddRecipeToBlackList(Guid recipeId)
	{
		var response = await httpClient.GetAsync("api/Admin/users");
		if (!response.IsSuccessStatusCode)
			return [];
		var paged = await response.Content.ReadFromJsonAsync<PagedResultDto<UserDto>>();
		return paged?.Items.ToList() ?? [];
	}

	public async Task<List<UserDto>> RemoveRecipeToBlackList(Guid recipeId)
	{
		var response = await httpClient.GetAsync("api/Admin/users");
		if (!response.IsSuccessStatusCode)
			return [];
		var paged = await response.Content.ReadFromJsonAsync<PagedResultDto<UserDto>>();
		return paged?.Items.ToList() ?? [];
	}
}
