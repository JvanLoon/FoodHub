namespace FoodCalc.Web.Components.Services;

/// <summary>
/// Fluent helpers that collapse the repetitive "check <see cref="ApiResult.Success"/>, pull
/// <see cref="ApiResult{T}.Data"/>, toast <see cref="ApiResult.Error"/>" dance at call sites
/// into a single expressive line. Every helper returns the same result so calls can be chained
/// or guarded on (<c>if (!result.Notify(messages).Success) return;</c>).
/// </summary>
public static class ApiResultExtensions
{
	// ==========================================
	// Asynchrone Overloads (voor Task-chaining)
	// ==========================================

	// ----- OnSuccess -----
	public static async Task<ApiResult> OnSuccess(this Task<ApiResult> resultTask, Action action, string? message = null)
	{
		var result = await resultTask;
		return result.OnSuccess(action, message);
	}

	public static async Task<ApiResult<T>> OnSuccess<T>(this Task<ApiResult<T>> resultTask, Action<T> action, string? message = null) where T : class
	{
		var result = await resultTask;
		return result.OnSuccess(action, message);
	}

	// ----- OnFailure -----
	public static async Task<ApiResult> OnFailure(this Task<ApiResult> resultTask, Action action, string? message = null)
	{
		var result = await resultTask;
		return result.OnFailure(action, message);
	}

	public static async Task<ApiResult<T>> OnFailure<T>(this Task<ApiResult<T>> resultTask, Action<string> action, string? message = null) where T : class
	{
		var result = await resultTask;
		return result.OnFailure(action, message);
	}

	// ----- Value extraction -----
	public static async Task<T> OrDefault<T>(this Task<ApiResult<T>> resultTask, T fallback, bool showMessage = false)
	{
		var result = await resultTask;
		return result.OrDefault(fallback, showMessage);
	}

	// ==========================================
	// Synchrone Basis (Moet PUBLIC zijn)
	// ==========================================

	/// <summary>Runs <paramref name="action"/> when the call succeeded.</summary>
	private static ApiResult OnSuccess(this ApiResult result, Action action, string? message = null)
	{
		if(message is not null)
			Notify(result, result.MessageService, message, action);

		return result;
	}

	/// <inheritdoc cref="OnSuccess(ApiResult, Action)"/>
	private static ApiResult<T> OnSuccess<T>(this ApiResult<T> result, Action<T> action, string? message = null) where T : class
	{
		if (message is not null)
			Notify(result, result.MessageService, message, () => action(result.Data!));

		return result;
	}

	/// <summary>Runs <paramref name="action"/> with the error message when the call failed.</summary>
	private static ApiResult OnFailure(this ApiResult result, Action action, string? message = null)
    {
		if (message is not null)
			NotifyFail(result, result.MessageService, message, action);

        return result;
    }

	private static ApiResult<T> OnFailure<T>(this ApiResult<T> result, Action<string> action, string? message = null) where T : class
	{
		if (message is not null)
			NotifyFail(result, result.MessageService, message, () => action(result.Error ?? "Onbekende fout"));

		return result;
	}

	// ----- Value extraction -----

	/// <summary>The payload on success, or <paramref name="fallback"/> on failure.</summary>
	private static T OrDefault<T>(this ApiResult<T> result, T fallback, bool showMessage = false)
	{
		if (showMessage)
			Notify(result, result.MessageService, result.Error);

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
	private static ApiResult Notify(this ApiResult result,
		MessageService? messageService = null,
		string? message = null,
		Action? action = null,
		int timeInMs = MessageService.DefaultDisplayTimeInMs)
	{
		if (result.Success)
		{
			if(!string.IsNullOrEmpty(message))
				messageService?.ShowMessage(message, isError: false, timeInMs);
			action?.Invoke();
		}
		else
		{
			if(!string.IsNullOrEmpty(result.Error))
				messageService?.ShowMessage(result.Error, isError: true, timeInMs);
		}

		return result;
	}

	/// <inheritdoc cref="Notify(ApiResult, MessageService, string?, int, int)"/>
	private static ApiResult<T> Notify<T>(this ApiResult<T> result,
		MessageService? messageService = null,
        string? message = null,
		Action? action = null,
		int timeInMs = MessageService.DefaultDisplayTimeInMs) where T : class 
    {
        // Cast to the base type so the non-generic overload runs (avoids recursion).
        ((ApiResult)result).Notify(messageService, message, action, timeInMs);
        return result;
    }

	/// <summary>
	/// On failure, shows the server's error message as an error toast. On success, shows
	/// <paramref name="message"/> as a success toast when one is supplied (otherwise stays silent).
	/// Returns the same result so it can be chained or guarded on.
	/// </summary>
	public static ApiResult NotifyFail(this ApiResult result,
		MessageService? messageService = null,
		string? message = null,
		Action? action = null,
		int timeInMs = MessageService.DefaultDisplayTimeInMs)
	{
		if (result.Success)
		{
			if(!string.IsNullOrEmpty(message))
				messageService?.ShowMessage(message, isError: false, timeInMs);
		}
		else
		{
			if(!string.IsNullOrEmpty(result.Error))
				messageService?.ShowMessage(result.Error, isError: true, timeInMs);
			action?.Invoke();
		}

		return result;
	}

	/// <summary>
	/// On failure, shows the server's error message as an error toast. On success, shows
	/// <paramref name="message"/> as a success toast when one is supplied (otherwise stays silent).
	/// Returns the same result so it can be chained or guarded on.
	/// </summary>
	private static ApiResult<T> NotifyFail<T>(this ApiResult<T> result,
		MessageService? messageService = null,
		string? message = null,
		Action? action = null,
		int timeInMs = MessageService.DefaultDisplayTimeInMs) where T : class
	{
		// Cast to the base type so the non-generic overload runs (avoids recursion).
		((ApiResult)result).NotifyFail(messageService, message, action, timeInMs);
		return result;
	}
}
