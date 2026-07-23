namespace FoodCalc.Web.Services;

/// <summary>
/// Minimal pub/sub for toast notifications. A single <c>ToastHost</c> subscribes per circuit
/// and owns all display and dismissal timing; this service just relays the request to it.
/// Showing a toast is fire-and-forget from the caller's point of view, so the API is synchronous.
/// </summary>
public class MessageService
{
	public const int DefaultDisplayTimeInMs = 12000;

	/// <summary>Raised when a toast should be shown. Subscribed to by <c>ToastHost</c>.</summary>
	public event Action<ToastMessage>? OnShowMessage;

	/// <param name="timeInMs">How long to show the toast; <c>0</c> keeps it until dismissed.</param>
	public void ShowMessage(string message,
							bool isError,
							int timeInMs = DefaultDisplayTimeInMs,
							CancellationToken cancellationToken = default
	) =>
		OnShowMessage?.Invoke(new ToastMessage(message, isError, timeInMs, cancellationToken));
}

/// <summary>A single toast request.</summary>
public readonly record struct ToastMessage(
	string Message,
	bool IsError,
	int TimeInMs,
	CancellationToken CancellationToken);
