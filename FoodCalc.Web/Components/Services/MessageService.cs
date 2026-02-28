namespace FoodCalc.Web.Components.Services;
public class MessageService
{
	public const int DefaultDisplayTimeInMs = 12000;
	public event Func<string, bool, int, Task>? OnShowMessage;

	public Task ShowMessageAsync(string message, bool isError, int timeInMs = DefaultDisplayTimeInMs)
	{
		if (OnShowMessage == null) return Task.CompletedTask;

		foreach (var handler in OnShowMessage.GetInvocationList().Cast<Func<string, bool, int, Task>>())
		{
			_ = InvokeHandlerAsync(handler, message, isError, timeInMs);
		}

		return Task.CompletedTask;
	}

	private static async Task InvokeHandlerAsync(Func<string, bool, int, Task> handler, string message, bool isError, int timeInMs)
	{
		try { await handler(message, isError, timeInMs); }
		catch { }
	}
}
