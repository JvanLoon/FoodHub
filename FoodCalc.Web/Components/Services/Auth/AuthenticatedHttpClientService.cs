using Blazored.LocalStorage;

using System.Net.Http.Headers;

namespace FoodCalc.Web.Components.Services.Auth;

public class AuthenticatedHttpClientService
{
	private readonly string _tokenName = "Authorization";
	private readonly HttpClient _httpClient;
	private readonly ILocalStorageService _localStorage;

	public AuthenticatedHttpClientService(HttpClient httpClient, ILocalStorageService localStorage)
	{
		_httpClient = httpClient;
		_localStorage = localStorage;
	}

	private async Task AttachTokenAsync()
	{
		var token = await _localStorage.GetItemAsync<string>(_tokenName);
		if (!string.IsNullOrWhiteSpace(token))
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
		else
		{
			_httpClient.DefaultRequestHeaders.Authorization = null;
		}
	}

	public async Task<HttpResponseMessage> GetAsync(string requestUri)
	{
		await AttachTokenAsync();
		return await _httpClient.GetAsync(requestUri);
	}

	public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
	{
		await AttachTokenAsync();
		return await _httpClient.PostAsJsonAsync(requestUri, value);
	}

	public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content = null)
	{
		await AttachTokenAsync();
		return await _httpClient.PostAsync(requestUri, content);
	}

	public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
	{
		await AttachTokenAsync();
		return await _httpClient.PutAsJsonAsync(requestUri, value);
	}

	public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
	{
		await AttachTokenAsync();
		return await _httpClient.DeleteAsync(requestUri);
	}
}
