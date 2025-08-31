using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using FoodHub.DTOs;

namespace FoodCalc.Web.Components.Services;
public class AdminService
{
    private readonly HttpClient _httpClient;

    public AdminService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>("/api/admin/users") ?? new List<UserDto>();
    }
}
