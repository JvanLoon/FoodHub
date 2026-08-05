using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FoodCalc.Web.Constants;
using Microsoft.AspNetCore.Components;

namespace FoodCalc.Web.Services.Auth;

/// <summary>
/// The single choke-point for talking to the API. Attaches the bearer token, sends the
/// request, and always returns an <see cref="ApiResult"/> — it never throws. On failure it
/// reads the response body to recover every message the server reported (RFC9457 ProblemDetails /
/// validation errors) and, for exceptions, logs the full stack trace to the console via
/// <see cref="ILogger"/>.
/// </summary>
public class AuthenticatedHttpClientService(
    HttpClient httpClient,
    TokenProvider tokenProvider,
    ILogger<AuthenticatedHttpClientService> logger,
    MessageService? messageService,
    NavigationManager navigation)
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Set once the session has been abandoned, so a page that fired several requests at once
    /// does not try to sign out and navigate several times over.
    /// </summary>
    private bool _sessionAbandoned;

    /// <summary>Attaches the bearer token, and reports whether there was one to attach.</summary>
    private async Task<bool> AttachTokenAsync()
    {
        var token = await tokenProvider.GetTokenAsync();
        var hasToken = !string.IsNullOrWhiteSpace(token);

        httpClient.DefaultRequestHeaders.Authorization = hasToken
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;

        return hasToken;
    }

    /// <summary>
    /// Handles a 401 that came back from a request we did send credentials on — the token has
    /// been refused, so it is finished. Before the API could revoke anything this only happened
    /// on expiry, which the client noticed for itself; now that disabling an account or changing
    /// its roles invalidates a token mid-session, the browser has no other way to find out. Left
    /// alone it would keep believing it was signed in, because the token has not expired, and
    /// every request would fail behind an error toast until it eventually did.
    ///
    /// Only fires when a token was actually attached, so a wrong password on the login form —
    /// also a 401 — is left to the page to report.
    ///
    /// Nothing can be cleared from in here any more. The credential is a cookie, and a cookie is
    /// dropped with a response header, which a circuit has none of. So this navigates to the
    /// logout route instead and lets a real request do it.
    /// </summary>
    private Task AbandonSessionAsync()
    {
        if (_sessionAbandoned)
            return Task.CompletedTask;

        _sessionAbandoned = true;

        try
        {
            // forceLoad, so this leaves the circuit rather than routing inside it — which is the
            // whole point, since only a real request can clear the cookie. It also tears the
            // circuit down, stopping the presence heartbeat and dropping any component still
            // holding state from the abandoned session.
            //
            // No offline ping first, unlike a deliberate sign-out: the token has just been
            // refused, so the ping could only 401 as well.
            navigation.NavigateTo(AuthCookie.LogoutPath, forceLoad: true);
        }
        catch (Exception ex)
        {
            // Navigating from a background timer (the heartbeat) is not always allowed. The
            // cookie then survives, but every request it authorises keeps coming back 401, and
            // the next one that reaches here from a real interaction will make the trip.
            logger.LogDebug(ex, "Could not redirect to the logout route after a rejected token");
        }

        return Task.CompletedTask;
    }

    // ----- Typed helpers returning ApiResult -----

    public Task<ApiResult<T>> GetAsync<T>(string requestUri) =>
        SendForDataAsync<T>(HttpMethod.Get, requestUri, content: null);

    /// <summary>GET where only success/failure matters and the response body is ignored.</summary>
    public Task<ApiResult> GetAsync(string requestUri) => SendAsync(HttpMethod.Get, requestUri, content: null);

    public Task<ApiResult> PostAsync<TRequest>(string requestUri, TRequest value) => SendAsync(HttpMethod.Post,
        requestUri, JsonContent.Create(value, options: _jsonOptions));

    public Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string requestUri, TRequest value) =>
        SendForDataAsync<TResponse>(HttpMethod.Post, requestUri, JsonContent.Create(value, options: _jsonOptions));

    /// <summary>
    /// POST raw <see cref="HttpContent"/> (query-string driven endpoints with no body, or custom
    /// content such as multipart file uploads). Named distinctly from the generic
    /// <see cref="PostAsync{TRequest}(string, TRequest)"/> on purpose: an <see cref="HttpContent"/>
    /// is an exact match for that generic's <c>TRequest</c>, so calling <c>PostAsync(uri, content)</c>
    /// would silently pick the JSON overload and serialize the content object itself as JSON.
    /// </summary>
    public Task<ApiResult> PostContentAsync(string requestUri, HttpContent? content = null) =>
        SendAsync(HttpMethod.Post, requestUri, content);

    public Task<ApiResult<TResponse>>
        PostContentForDataAsync<TResponse>(string requestUri, HttpContent? content = null) =>
        SendForDataAsync<TResponse>(HttpMethod.Post, requestUri, content);

    public Task<ApiResult> PutAsync<TRequest>(string requestUri, TRequest value) => SendAsync(HttpMethod.Put,
        requestUri, JsonContent.Create(value, options: _jsonOptions));

    public Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string requestUri, TRequest value) =>
        SendForDataAsync<TResponse>(HttpMethod.Put, requestUri, JsonContent.Create(value, options: _jsonOptions));

    public Task<ApiResult> DeleteAsync(string requestUri) => SendAsync(HttpMethod.Delete, requestUri, content: null);

    public Task<ApiResult<TResponse>> DeleteAsync<TResponse>(string requestUri) =>
        SendForDataAsync<TResponse>(HttpMethod.Delete, requestUri, content: null);

    // ----- Core send logic -----

    private async Task<ApiResult> SendAsync(HttpMethod method, string requestUri, HttpContent? content)
    {
        try
        {
            var (response, tokenAttached) = await SendRawAsync(method, requestUri, content);
            using (response)
            {
                if (tokenAttached && response.StatusCode == HttpStatusCode.Unauthorized)
                    await AbandonSessionAsync();

                var result = response.IsSuccessStatusCode
                    ? ApiResult.Ok((int) response.StatusCode)
                    : ApiResult.Fail(await ExtractErrorsAsync(response, method, requestUri),
                        (int) response.StatusCode);
                return await DecorateAsync(result);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request {Method} {Uri} failed", method, requestUri);
            return await DecorateAsync(ApiResult.Fail(WebConstants.Messages.Client.GenericFailure,
                (int) HttpStatusCode.InternalServerError));
        }
    }

    private async Task<ApiResult<T>> SendForDataAsync<T>(HttpMethod method, string requestUri, HttpContent? content)
    {
        try
        {
            var (response, tokenAttached) = await SendRawAsync(method, requestUri, content);
            using (response)
            {
                if (tokenAttached && response.StatusCode == HttpStatusCode.Unauthorized)
                    await AbandonSessionAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errors = await ExtractErrorsAsync(response, method, requestUri);
                    return await DecorateAsync(ApiResult<T>.Fail(errors, (int) response.StatusCode));
                }

                var data = await ReadDataAsync<T>(response);
                return await DecorateAsync(ApiResult<T>.Ok(data!, (int) response.StatusCode));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request {Method} {Uri} failed", method, requestUri);
            return await DecorateAsync(ApiResult<T>.Fail(WebConstants.Messages.Client.GenericFailure,
                (int) HttpStatusCode.InternalServerError));
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

    /// <summary>
    /// Sends the request and reports whether it carried a token, which is what tells a 401
    /// "your session is finished" apart from a 401 that simply means "wrong password".
    /// </summary>
    private async Task<(HttpResponseMessage Response, bool TokenAttached)> SendRawAsync(HttpMethod method,
        string requestUri,
        HttpContent? content)
    {
        var tokenAttached = await AttachTokenAsync();

        // A POST/PUT/PATCH with no body carries no Content-Type either, and the API
        // answers 415 Unsupported Media Type before the endpoint ever runs — which
        // surfaced in the UI as the UnsupportedMediaType fallback. The endpoints this
        // affects take everything from the query string (ToggleUser, UserRoles), so
        // an empty JSON object satisfies the model binder without changing what is
        // sent. DELETE is left alone: it is legitimately bodyless.
        if (content is null && method != HttpMethod.Delete)
            content = new StringContent("{}", Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(method, requestUri)
        {
            Content = content
        };
        return (await httpClient.SendAsync(request), tokenAttached);
    }

    /// <summary>Deserialize the success body. Handles raw string endpoints and empty bodies.</summary>
    private static async Task<T?> ReadDataAsync<T>(HttpResponseMessage response)
    {
        if (typeof(T) == typeof(string))
        {
            var text = await response.Content.ReadAsStringAsync();
            return (T) (object) text;
        }

        if (response.Content.Headers.ContentLength == 0)
            return default;

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    /// <summary>
    /// Turn a non-success response into clean, user-ready messages — <em>all</em> of them, since
    /// the API reports every error it found. Prefers the server's own wording (RFC9457
    /// ProblemDetails <c>errors</c>, <c>detail</c>, <c>title</c>/<c>message</c>), falling back to a
    /// status-based message. Logged at Warning level.
    /// </summary>
    private async Task<IReadOnlyList<string>> ExtractErrorsAsync(HttpResponseMessage response,
        HttpMethod method,
        string requestUri)
    {
        string? body = null;
        try { body = await response.Content.ReadAsStringAsync(); }
        catch
        {
            // ignore — fall back to status-based message below
        }

        var messages = ParseServerMessages(body);
        if (messages.Count == 0)
            messages = [StatusFallback(response.StatusCode)];

        messages.ForEach(message => logger.LogWarning("Request {Method} {Uri} returned {Status}: {Message}", method,
            requestUri, (int) response.StatusCode, message));

        return messages;
    }

    /// <summary>
    /// Pull every message out of an error body. Returns an empty list when the body yields nothing
    /// usable, so the caller can fall back to a status-based message.
    /// </summary>
    private static List<string> ParseServerMessages(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return [];

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                return [Trim(body)];

            if (root.TryGetProperty("errors", out var errors))
            {
                var messages = ReadErrors(errors);
                if (messages.Count > 0)
                    return messages;
            }

            // Single-message shapes used by ProblemDetails and various handlers.
            foreach (var name in new[] { "detail", "title", "message", "error" })
            {
                if (root.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
                {
                    var value = prop.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                        return [value];
                }
            }

            return [];
        }
        catch (JsonException)
        {
            // Not JSON — a plain text/plain body. Use it directly if it's reasonable.
            return [Trim(body)];
        }
    }

    /// <summary>
    /// Read the <c>errors</c> member, which comes in two shapes:
    /// <list type="bullet">
    /// <item>RFC9457 ProblemDetails (what the API sends now): <c>[{ "name": .., "reason": .. }]</c>.</item>
    /// <item>Classic model-state style: <c>{ "Field": ["msg", ..] }</c> or <c>{ "Field": "msg" }</c>.</item>
    /// </list>
    /// </summary>
    private static List<string> ReadErrors(JsonElement errors)
    {
        var messages = new List<string>();

        if (errors.ValueKind == JsonValueKind.Array)
        {
            foreach (var error in errors.EnumerateArray())
            {
                if (error.ValueKind == JsonValueKind.String)
                    AddIfPresent(error.GetString());
                else if (error.ValueKind == JsonValueKind.Object && error.TryGetProperty("reason", out var reason))
                    AddIfPresent(reason.GetString());
            }
        }
        else if (errors.ValueKind == JsonValueKind.Object)
        {
            foreach (var field in errors.EnumerateObject())
            {
                if (field.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var entry in field.Value.EnumerateArray())
                        if (entry.ValueKind == JsonValueKind.String)
                            AddIfPresent(entry.GetString());
                }
                else if (field.Value.ValueKind == JsonValueKind.String) { AddIfPresent(field.Value.GetString()); }
            }
        }

        return messages;

        void AddIfPresent(string? message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                messages.Add(message);
        }
    }

    private static string Trim(string body) => body.Length > 500 ? body[..500] : body;

    private static string StatusFallback(HttpStatusCode status) => status switch
    {
        // --- 2xx Success ---
        HttpStatusCode.OK => WebConstants.Messages.Client.OK,
        HttpStatusCode.Created => WebConstants.Messages.Client.Created,
        HttpStatusCode.Accepted => WebConstants.Messages.Client.Accepted,
        HttpStatusCode.NoContent => WebConstants.Messages.Client.NoContent,

        // --- 4xx Client Errors ---
        HttpStatusCode.BadRequest => WebConstants.Messages.Client.BadRequest,
        HttpStatusCode.Unauthorized => WebConstants.Messages.Client.Unauthorized,
        HttpStatusCode.Forbidden => WebConstants.Messages.Client.Forbidden,
        HttpStatusCode.NotFound => WebConstants.Messages.Client.NotFound,
        HttpStatusCode.RequestTimeout => WebConstants.Messages.Client.RequestTimeout,
        HttpStatusCode.Conflict => WebConstants.Messages.Client.Conflict,
        HttpStatusCode.UnsupportedMediaType => WebConstants.Messages.Client.UnsupportedMediaType,
        HttpStatusCode.TooManyRequests => WebConstants.Messages.Client.TooManyRequests,
        HttpStatusCode.RequestEntityTooLarge => WebConstants.Messages.Client.PayloadTooLarge,
        HttpStatusCode.MethodNotAllowed => WebConstants.Messages.Client.MethodNotAllowed,
        HttpStatusCode.UnprocessableEntity => WebConstants.Messages.Client.UnprocessableEntity,

        // --- 5xx Server Errors ---
        HttpStatusCode.InternalServerError => WebConstants.Messages.Client.InternalServerError,
        HttpStatusCode.NotImplemented => WebConstants.Messages.Client.NotImplemented,
        HttpStatusCode.BadGateway => WebConstants.Messages.Client.BadGateway,
        HttpStatusCode.ServiceUnavailable => WebConstants.Messages.Client.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout => WebConstants.Messages.Client.GatewayTimeout,
        HttpStatusCode.NetworkAuthenticationRequired => WebConstants.Messages.Client.NetworkAuthenticationRequired,

        // --- Fallback voor onbekende of custom statuscodes ---
        _ => WebConstants.Messages.Client.RequestFailed((int) status)
    };
}