using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services
{
    public class AdminService(HttpClient httpClient)
	{
        public async Task<List<UserDto>> GetUsersAsync()
        {
            return await httpClient.GetFromJsonAsync<List<UserDto>>("/api/admin/users") ?? new List<UserDto>();
        }
    }
}
