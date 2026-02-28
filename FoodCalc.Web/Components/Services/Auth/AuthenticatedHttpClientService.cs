using System.Net.Http.Headers;

namespace FoodCalc.Web.Components.Services.Auth;

public class AuthenticatedHttpClientService(HttpClient httpClient, AuthTokenService authTokenService)
{
    private async Task AttachTokenAsync()
    {
        var token = await authTokenService.GetTokenAsync();
        httpClient.DefaultRequestHeaders.Authorization = !string.IsNullOrWhiteSpace(token)
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        await AttachTokenAsync();
        return await httpClient.GetAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
    {
        await AttachTokenAsync();
        return await httpClient.PostAsJsonAsync(requestUri, value);
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content = null)
    {
        await AttachTokenAsync();
        return await httpClient.PostAsync(requestUri, content);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
    {
        await AttachTokenAsync();
        return await httpClient.PutAsJsonAsync(requestUri, value);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        await AttachTokenAsync();
        return await httpClient.DeleteAsync(requestUri);
    }
}
