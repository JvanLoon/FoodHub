using Microsoft.AspNetCore.Mvc;

using static FoodCalc.Web.Components.Constants.WebConstants;

namespace FoodCalc.Web.Components.Services;

/// <summary>
/// Fluent helpers that collapse the repetitive "check <see cref="ApiResult.Success"/>, pull
/// <see cref="ApiResult{T}.Data"/>, toast <see cref="ApiResult.Error"/>" dance at call sites
/// into a single expressive line. Every helper returns the same result so calls can be chained
/// or guarded on (<c>if (!result.Notify(messages).Success) return;</c>).
/// </summary>
public static class ApiResultExtensions
{
	// ----- Side effects on the outcome -----

	/// <summary>Runs <paramref name="action"/> when the call succeeded.</summary>
	public static ApiResult OnSuccess(this ApiResult result, Action action, string? message = null)
	{
		Notify(result, result.MessageService, message, action);
		return result;
	}

	/// <inheritdoc cref="OnSuccess(ApiResult, Action)"/>
	public static ApiResult<T> OnSuccess<T>(this ApiResult<T> result, Action<T> action, string? message = null) where T : class
	{
		Notify(result, result.MessageService, message, () => action(result.Data!));
		return result;
	}

    /// <summary>Runs <paramref name="action"/> with the error message when the call failed.</summary>
    public static ApiResult OnFailure(this ApiResult result, Action action, string? message = null)
    {
		NotifyFail(result, result.MessageService, message, action);
        return result;
    }

	// ----- Value extraction -----

	/// <summary>The payload on success, or <paramref name="fallback"/> on failure.</summary>
	public static T OrDefault<T>(this ApiResult<T> result, T fallback, bool showMessage = false)
	{
		if(result.Success)
			return result.Data!;

		if (showMessage)
			Notify(result, result.MessageService, result.Error);

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
	private static ApiResult NotifyFail(this ApiResult result,
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
}
