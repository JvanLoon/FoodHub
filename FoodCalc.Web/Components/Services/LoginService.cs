using FoodHub.DTOs;

using Microsoft.AspNetCore.Components;

using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FoodCalc.Web.Components.Services
{
    public class LoginService(HttpClient httpClient)
	{
        public async Task<HttpResponseMessage> LoginAsync(LoginDto user)
        {
			return await httpClient.PostAsJsonAsync("api/authentication/login", user);
        }

        public async Task<bool> RegisterAsync(RegisterDto user)
        {
            var response = await httpClient.PostAsJsonAsync("/api/authentication/register", user);
            return response.IsSuccessStatusCode;
        }
    }
}
