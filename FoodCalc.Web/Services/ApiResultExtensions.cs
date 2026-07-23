namespace FoodCalc.Web.Services;

/// <summary>
/// Fluent helpers that collapse the repetitive "check <see cref="ApiResult.Success"/>, pull
/// <see cref="ApiResult{T}.Data"/>, toast <see cref="ApiResult.Errors"/>" dance at call sites
/// into a single expressive line. Every helper returns the same result so calls can be chained
/// or guarded on (<c>if (!result.Notify(messages).Success) return;</c>).
/// </summary>
public static class ApiResultExtensions
{
	private const int _timeInMs = MessageService.DefaultDisplayTimeInMs;
	// ==========================================
	// Asynchrone Overloads (voor Task-chaining)
	// ==========================================

	// ----- OnSuccess -----
	public static async Task<ApiResult> OnSuccess(this Task<ApiResult> resultTask,
												  Action? action = null,
												  string? message = null
	)
	{
		var result = await resultTask;
		return result.OnSuccess(action, message);
	}

	public static async Task<ApiResult<T>> OnSuccess<T>(this Task<ApiResult<T>> resultTask,
														Action<T>? action = null,
														string? message = null
	) where T : class
	{
		var result = await resultTask;
		return result.OnSuccess(action, message);
	}

	// ----- OnFailure -----
	public static async Task<ApiResult> OnFailure(this Task<ApiResult> resultTask, Action? action = null)
	{
		var result = await resultTask;
		return result.OnFailure(action);
	}

	public static async Task<ApiResult<T>> OnFailure<T>(this Task<ApiResult<T>> resultTask,
														Action<IReadOnlyList<string>>? action = null
	) where T : class
	{
		var result = await resultTask;
		return result.OnFailure(action);
	}

	// ----- Value extraction -----
	public static async Task<T> OrDefault<T>(this Task<ApiResult<T>> resultTask, T fallback)
	{
		var result = await resultTask;
		return result.OrDefault(fallback);
	}

	// ==========================================
	// Synchrone Basis (Moet PUBLIC zijn)
	// ==========================================

	/// <summary>Runs <paramref name="action"/> when the call succeeded.</summary>
	private static ApiResult OnSuccess(this ApiResult result, Action? action, string? message = null)
	{
		NotifySuccess(result, message, action);

		return result;
	}

	/// <inheritdoc cref="OnSuccess(ApiResult, Action)"/>
	private static ApiResult<T> OnSuccess<T>(this ApiResult<T> result, Action<T>? action, string? message = null)
		where T : class
	{
		NotifySuccess(result, message, () => action?.Invoke(result.Data!));

		return result;
	}

	/// <summary>Runs <paramref name="action"/> with the error message when the call failed.</summary>
	private static ApiResult OnFailure(this ApiResult result, Action? action)
	{
		NotifyFail(result, action);

		return result;
	}

	private static ApiResult<T> OnFailure<T>(this ApiResult<T> result,
											 Action<IReadOnlyList<string>>? action,
											 string? message = null
	) where T : class
	{
		NotifyFail(result, () => action?.Invoke(result.Errors));

		return result;
	}

	// ----- Value extraction -----

	/// <summary>The payload on success, or <paramref name="fallback"/> on failure.</summary>
	private static T OrDefault<T>(this ApiResult<T> result, T fallback)
	{
		NotifyFail(result);

		if (result.Success)
			return result.Data!;

		return fallback;
	}

	// ----- Toast the outcome via MessageService -----

	/// <summary>
	/// On failure, shows the server's error message as an error toast. On success, shows
	/// <paramref name="message"/> as a success toast when one is supplied (otherwise stays silent).
	/// Returns the same result so it can be chained or guarded on.
	/// </summary>
	private static ApiResult NotifySuccess(this ApiResult result, string? message = null, Action? action = null)
	{
		if (result.Success)
		{
			if (!string.IsNullOrEmpty(message))
				result.MessageService?.ShowMessage(message, isError: false, _timeInMs);

			action?.Invoke();
		}

		return result;
	}

	/// <inheritdoc cref="NotifySuccess(ApiResult, MessageService, string?, int, int)"/>
	private static ApiResult<T> NotifySuccess<T>(this ApiResult<T> result,
												 string? message = null,
												 Action? action = null
	) where T : class
	{
		// Cast to the base type so the non-generic overload runs (avoids recursion).
		((ApiResult) result).NotifySuccess(message, action);
		return result;
	}

	/// <summary>
	/// On failure, shows one error toast per message the server reported — usually a single one,
	/// but endpoints may return several and none of them should be swallowed.
	/// Returns the same result so it can be chained or guarded on.
	/// </summary>
	private static ApiResult NotifyFail(this ApiResult result, Action? action = null)
	{
		if (!result.Success)
		{
			foreach (var error in result.Errors)
			{
				if (!string.IsNullOrEmpty(error))
					result.MessageService?.ShowMessage(error, isError: true, _timeInMs);
			}

			action?.Invoke();
		}

		return result;
	}

	/// <summary>
	/// On failure, shows the server's error message as an error toast. On success, shows
	/// <paramref name="message"/> as a success toast when one is supplied (otherwise stays silent).
	/// Returns the same result so it can be chained or guarded on.
	/// </summary>
	private static ApiResult<T> NotifyFail<T>(this ApiResult<T> result, Action? action = null) where T : class
	{
		// Cast to the base type so the non-generic overload runs (avoids recursion).
		((ApiResult) result).NotifyFail(action);
		return result;
	}
}
