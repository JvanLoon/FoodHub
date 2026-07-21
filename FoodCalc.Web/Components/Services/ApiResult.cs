namespace FoodCalc.Web.Components.Services;

/// <summary>
/// Outcome of an API call. Either succeeded (optionally with data) or failed with a
/// clean, user-ready <see cref="Error"/> message. Services return this instead of
/// bool / HttpResponseMessage so every caller handles success and failure the same way.
/// The underlying exception (if any) is logged to the console by the HTTP client, not
/// surfaced here.
/// </summary>
public class ApiResult
{
	internal MessageService? MessageService { get; set; }

	public bool Success { get; init; }

    /// <summary>Clean, user-ready message. Null on success.</summary>
    public string? Error { get; init; }

    /// <summary>HTTP status code, when one is available (0 for transport/exception failures).</summary>
    public int StatusCode { get; init; }

    public static ApiResult Ok(int statusCode = 200) =>
        new() { Success = true, StatusCode = statusCode };

    public static ApiResult Fail(string error, int statusCode = 0) =>
        new() { Success = false, Error = error, StatusCode = statusCode };
}

/// <summary>An <see cref="ApiResult"/> that carries a payload on success.</summary>
public class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }

    public static ApiResult<T> Ok(T data, int statusCode = 200) =>
        new() { Success = true, Data = data, StatusCode = statusCode };

    public static new ApiResult<T> Fail(string error, int statusCode = 0) =>
        new() { Success = false, Error = error, StatusCode = statusCode };
}
