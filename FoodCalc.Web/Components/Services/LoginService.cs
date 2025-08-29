using FoodHub.DTOs;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.ComponentModel.DataAnnotations;

using static FoodCalc.Web.Components.Pages.Login;
using static System.Net.WebRequestMethods;

namespace FoodCalc.Web.Components.Services
{
	public class LoginService(HttpClient httpClient, NavigationManager navigationManager)
	{
		public async Task<bool> Login(LoginModel loginModel)
		{
			var response = await httpClient.PostAsJsonAsync("api/auth/login", loginModel);
			if (response.IsSuccessStatusCode)
			{
				navigationManager.NavigateTo("/", true);
				//Navigation.NavigateTo("/home");
				return true;
			}

			return false;
		}

		public async Task<bool> Logout()
		{
			var response = await httpClient.PostAsync("api/auth/logout", null);
			if (response.IsSuccessStatusCode)
			{
				navigationManager.NavigateTo("/", true);
				//Navigation.NavigateTo("/home");
				return true;
			}

			return false;
		}
	}
}

public class LoginModel
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }
	[Required]
	public string Password { get; set; }
}
