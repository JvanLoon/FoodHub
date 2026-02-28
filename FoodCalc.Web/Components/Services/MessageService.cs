namespace FoodCalc.Web.Components.Services;

public class MessageService
{
    public const int DefaultDisplayTimeInMs = 12000;
    public event Func<string, bool, int, CancellationToken, Task>? OnShowMessage;

    public Task ShowMessageAsync(string message, bool isError, int timeInMs = DefaultDisplayTimeInMs, CancellationToken cancellationToken = default)
    {
        if (OnShowMessage == null) return Task.CompletedTask;

        foreach (var handler in OnShowMessage.GetInvocationList().Cast<Func<string, bool, int, CancellationToken, Task>>())
        {
            _ = InvokeHandlerAsync(handler, message, isError, timeInMs, cancellationToken);
        }

        return Task.CompletedTask;
    }

    private static async Task InvokeHandlerAsync(Func<string, bool, int, CancellationToken, Task> handler, string message, bool isError, int timeInMs, CancellationToken cancellationToken)
    {
        try {
			await handler(message, isError, timeInMs, cancellationToken);
		}
        catch (OperationCanceledException) { }
        catch { }
    }
}
