namespace FoodCalc.Web.Components.Services;

/// <summary>
/// Outcome of an API call. Either succeeded (optionally with data) or failed with one or more
/// clean, user-ready messages in <see cref="Errors"/>. Services return this instead of
/// bool / HttpResponseMessage so every caller handles success and failure the same way.
/// The underlying exception (if any) is logged to the console by the HTTP client, not
/// surfaced here.
/// </summary>
public class ApiResult
{
	internal MessageService? MessageService { get; set; }

	public bool Success { get; init; }

	/// <summary>
	/// Clean, user-ready messages — every error the server reported, not just the first.
	/// Empty on success.
	/// </summary>
	public IReadOnlyList<string> Errors { get; init; } = [];

	/// <summary>HTTP status code, when one is available (0 for transport/exception failures).</summary>
	public int StatusCode { get; init; }

	public static ApiResult Ok(int statusCode = 200) =>
		new() { Success = true, StatusCode = statusCode };

	public static ApiResult Fail(string error, int statusCode = 0) =>
		Fail([$"{statusCode} {error}"], statusCode);

	public static ApiResult Fail(IReadOnlyList<string> errors, int statusCode = 0) =>
		new() { Success = false, Errors = errors, StatusCode = statusCode };
}

/// <summary>An <see cref="ApiResult"/> that carries a payload on success.</summary>
public class ApiResult<T> : ApiResult
{
	public T? Data { get; init; }

	public static ApiResult<T> Ok(T data, int statusCode = 200) =>
		new() { Success = true, Data = data, StatusCode = statusCode };

	public static new ApiResult<T> Fail(string error, int statusCode = 0) =>
		Fail([$"{statusCode} {error}"], statusCode);

	public static new ApiResult<T> Fail(IReadOnlyList<string> errors, int statusCode = 0) =>
		new() { Success = false, Errors = errors, StatusCode = statusCode };
}
