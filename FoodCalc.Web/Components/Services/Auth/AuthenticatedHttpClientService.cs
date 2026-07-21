using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using FoodCalc.Web.Components.Services;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Web.Components.Services.Auth;

/// <summary>
/// The single choke-point for talking to the API. Attaches the bearer token, sends the
/// request, and always returns an <see cref="ApiResult"/> — it never throws. On failure it
/// reads the response body to recover the server's real message (ProblemDetails / validation
/// errors) and, for exceptions, logs the full stack trace to the console via <see cref="ILogger"/>.
/// </summary>
public class AuthenticatedHttpClientService(
    HttpClient httpClient,
    AuthTokenService authTokenService,
    ILogger<AuthenticatedHttpClientService> logger,
	MessageService? messageService)
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    private async Task AttachTokenAsync()
    {
        var token = await authTokenService.GetTokenAsync();
        httpClient.DefaultRequestHeaders.Authorization = !string.IsNullOrWhiteSpace(token)
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;
    }

    // ----- Typed helpers returning ApiResult -----

    public Task<ApiResult<T>> GetAsync<T>(string requestUri) =>
        SendForDataAsync<T>(HttpMethod.Get, requestUri, content: null);

    public Task<ApiResult> PostAsync<TRequest>(string requestUri, TRequest value) =>
        SendAsync(HttpMethod.Post, requestUri, JsonContent.Create(value, options: _jsonOptions));

    public Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string requestUri, TRequest value) =>
        SendForDataAsync<TResponse>(HttpMethod.Post, requestUri, JsonContent.Create(value, options: _jsonOptions));

    /// <summary>POST with no JSON body (query-string driven endpoints, or custom content such as multipart).</summary>
    public Task<ApiResult> PostAsync(string requestUri, HttpContent? content = null) =>
        SendAsync(HttpMethod.Post, requestUri, content);

    public Task<ApiResult<TResponse>> PostForDataAsync<TResponse>(string requestUri, HttpContent? content = null) =>
        SendForDataAsync<TResponse>(HttpMethod.Post, requestUri, content);

    public Task<ApiResult> PutAsync<TRequest>(string requestUri, TRequest value) =>
        SendAsync(HttpMethod.Put, requestUri, JsonContent.Create(value, options: _jsonOptions));

    public Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string requestUri, TRequest value) =>
        SendForDataAsync<TResponse>(HttpMethod.Put, requestUri, JsonContent.Create(value, options: _jsonOptions));

    public Task<ApiResult> DeleteAsync(string requestUri) =>
        SendAsync(HttpMethod.Delete, requestUri, content: null);

    public Task<ApiResult<TResponse>> DeleteAsync<TResponse>(string requestUri) =>
        SendForDataAsync<TResponse>(HttpMethod.Delete, requestUri, content: null);

    // ----- Core send logic -----

    private async Task<ApiResult> SendAsync(HttpMethod method, string requestUri, HttpContent? content)
    {
        try
        {
            using var response = await SendRawAsync(method, requestUri, content);
            var result = response.IsSuccessStatusCode
                ? ApiResult.Ok((int)response.StatusCode)
                : ApiResult.Fail(await ExtractErrorAsync(response, method, requestUri), (int)response.StatusCode);
            return await DecorateAsync(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request {Method} {Uri} failed", method, requestUri);
            return await DecorateAsync(ApiResult.Fail(WebConstants.Messages.Client.GenericFailure));
        }
    }

    private async Task<ApiResult<T>> SendForDataAsync<T>(HttpMethod method, string requestUri, HttpContent? content)
    {
        try
        {
            using var response = await SendRawAsync(method, requestUri, content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await ExtractErrorAsync(response, method, requestUri);
                return await DecorateAsync(ApiResult<T>.Fail(error, (int)response.StatusCode));
            }

            var data = await ReadDataAsync<T>(response);
            return await DecorateAsync(ApiResult<T>.Ok(data!, (int)response.StatusCode));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request {Method} {Uri} failed", method, requestUri);
            return await DecorateAsync(ApiResult<T>.Fail(WebConstants.Messages.Client.GenericFailure));
        }
    }

    /// <summary>
    /// Stamps UI context onto a result before it reaches the fluent notify helpers: the
    /// <see cref="MessageService"/> that shows toasts, and whether the current user is an admin
    /// (so raw server error messages are only surfaced to admins).
    /// </summary>
    private async Task<TResult> DecorateAsync<TResult>(TResult result) where TResult : ApiResult
    {
        result.MessageService = messageService;
        return result;
    }

    private async Task<HttpResponseMessage> SendRawAsync(HttpMethod method, string requestUri, HttpContent? content)
    {
        await AttachTokenAsync();
        using var request = new HttpRequestMessage(method, requestUri) { Content = content };
        return await httpClient.SendAsync(request);
    }

    /// <summary>Deserialize the success body. Handles raw string endpoints and empty bodies.</summary>
    private static async Task<T?> ReadDataAsync<T>(HttpResponseMessage response)
    {
        if (typeof(T) == typeof(string))
        {
            var text = await response.Content.ReadAsStringAsync();
            return (T)(object)text;
        }

        if (response.Content.Headers.ContentLength == 0)
            return default;

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    /// <summary>
    /// Turn a non-success response into a clean, user-ready message. Prefers the server's own
    /// wording (ProblemDetails <c>detail</c>, FastEndpoints validation <c>errors</c>, <c>title</c>/
    /// <c>message</c>), falling back to a status-based message. Logged at Warning level.
    /// </summary>
    private async Task<string> ExtractErrorAsync(HttpResponseMessage response, HttpMethod method, string requestUri)
    {
        string? body = null;
        try
        {
            body = await response.Content.ReadAsStringAsync();
        }
        catch
        {
            // ignore — fall back to status-based message below
        }

        var message = ParseServerMessage(body) ?? StatusFallback(response.StatusCode);

        logger.LogWarning("Request {Method} {Uri} returned {Status}: {Message}",
            method, requestUri, (int)response.StatusCode, message);

        return message;
    }

    private static string? ParseServerMessage(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                return Trim(body);

            // ProblemDetails (TypedResults.Problem) -> "detail"
            if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
            {
                var value = detail.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            // Validation failures -> "errors": { field: [msg, ...] } or { field: "msg" }
            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                var messages = new List<string>();
                foreach (var field in errors.EnumerateObject())
                {
                    if (field.Value.ValueKind == JsonValueKind.Array)
                        messages.AddRange(field.Value.EnumerateArray()
                            .Where(e => e.ValueKind == JsonValueKind.String)
                            .Select(e => e.GetString()!));
                    else if (field.Value.ValueKind == JsonValueKind.String)
                        messages.Add(field.Value.GetString()!);
                }

                if (messages.Count > 0)
                    return string.Join(" ", messages);
            }

            // Fallbacks used by various handlers
            foreach (var name in new[] { "title", "message", "error" })
            {
                if (root.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
                {
                    var value = prop.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                        return value;
                }
            }

            return null;
        }
        catch (JsonException)
        {
            // Not JSON — a plain text/plain body. Use it directly if it's reasonable.
            return Trim(body);
        }
    }

    private static string Trim(string body) =>
        body.Length > 500 ? body[..500] : body;

    private static string StatusFallback(HttpStatusCode status) => status switch
    {
        HttpStatusCode.Unauthorized => WebConstants.Messages.Client.Unauthorized,
        HttpStatusCode.Forbidden => WebConstants.Messages.Client.Forbidden,
        HttpStatusCode.NotFound => WebConstants.Messages.Client.NotFound,
        HttpStatusCode.BadRequest => WebConstants.Messages.Client.BadRequest,
        HttpStatusCode.Conflict => WebConstants.Messages.Client.Conflict,
        HttpStatusCode.InternalServerError => WebConstants.Messages.Client.ServerError,
        _ => WebConstants.Messages.Client.RequestFailed((int)status)
    };
}
