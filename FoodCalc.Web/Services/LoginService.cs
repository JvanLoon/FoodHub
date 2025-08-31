using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FoodCalc.Web.Services
{
    public class LoginService
    {
        private readonly HttpClient _http;
        public LoginService(HttpClient http)
        {
            _http = http;
        }

        public async Task<AuthResponseDto?> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7426/api/authentication/login", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            return null;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7426/api/authentication/register", new { Email = email, Password = password });
            return response.IsSuccessStatusCode;
        }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
    }
}
