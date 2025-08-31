using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FoodCalc.Web.Services
{
    public class AdminService
    {
        private readonly HttpClient _http;
        public AdminService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            return await _http.GetFromJsonAsync<List<UserDto>>("https://localhost:7426/api/admin/users") ?? new List<UserDto>();
        }
    }

    public class UserDto
    {
        public string Email { get; set; }
        public bool Enabled { get; set; }
    }
}
