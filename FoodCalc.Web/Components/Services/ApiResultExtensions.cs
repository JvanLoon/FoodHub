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
	public static ApiResult OnSuccess(this ApiResult result, Action action)
	{
		if (result.Success) action();
		return result;
	}

	/// <inheritdoc cref="OnSuccess(ApiResult, Action)"/>
	public static ApiResult<T> OnSuccess<T>(this ApiResult<T> result, Action<T> action)
	{
		if (result.Success) action(result.Data!);
		return result;
	}

	/// <summary>Runs <paramref name="action"/> when the call succeeded, without the payload.</summary>
	public static ApiResult Then(this ApiResult result, Action action)
    {
        if (result.Success) action();
        return result;
    }

    /// <inheritdoc cref="Then(ApiResult, Action)"/>
    public static ApiResult<T> Then<T>(this ApiResult<T> result, Action action)
    {
        if (result.Success) action();
        return result;
    }

    /// <summary>Runs <paramref name="action"/> with the error message when the call failed.</summary>
    public static ApiResult OnFailure(this ApiResult result, Action<string> action)
    {
        if (!result.Success) action(result.Error!);
        return result;
    }

    /// <inheritdoc cref="OnFailure(ApiResult, Action{string})"/>
    public static ApiResult<T> OnFailure<T>(this ApiResult<T> result, Action<string> action)
    {
        if (!result.Success) action(result.Error!);
        return result;
    }

    // ----- Toast the outcome via MessageService -----

	/// <summary>
	/// On failure, shows the server's error message as an error toast. On success, shows
	/// <paramref name="success"/> as a success toast when one is supplied (otherwise stays silent).
	/// Returns the same result so it can be chained or guarded on.
	/// </summary>
	public static ApiResult Notify(this ApiResult result, MessageService messages,
        string? success = null,
        int successTimeInMs = MessageService.DefaultDisplayTimeInMs,
        int errorTimeInMs = MessageService.DefaultDisplayTimeInMs)
    {
        if (result.Success)
        {
            if (success is not null)
                messages.ShowMessage(success, isError: false, successTimeInMs);
        }
        else
        {
            messages.ShowMessage(result.Error!, isError: true, errorTimeInMs);
        }

        return result;
    }

    /// <inheritdoc cref="Notify(ApiResult, MessageService, string?, int, int)"/>
    public static ApiResult<T> Notify<T>(this ApiResult<T> result, MessageService messages,
        string? success = null,
        int successTimeInMs = MessageService.DefaultDisplayTimeInMs,
        int errorTimeInMs = MessageService.DefaultDisplayTimeInMs)
    {
        // Cast to the base type so the non-generic overload runs (avoids recursion).
        ((ApiResult)result).Notify(messages, success, successTimeInMs, errorTimeInMs);
        return result;
    }

    // ----- Value extraction -----

    /// <summary>The payload on success, or <paramref name="fallback"/> on failure.</summary>
    public static T OrDefault<T>(this ApiResult<T> result, T fallback) =>
        result.Success ? result.Data! : fallback;
}
