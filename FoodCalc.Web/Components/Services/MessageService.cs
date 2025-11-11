using System;
using System.Threading.Tasks;

//namespace FoodCalc.Web.Components.Services;
public class MessageService
{
	public const int DefaultDisplayTimeInMs = 12000;
	public event Func<string, bool, int, Task>? OnShowMessage;

	public async Task ShowMessageAsync(string message, bool isError, int timeInMs = DefaultDisplayTimeInMs)
	{
		if (OnShowMessage != null)
		{
			await OnShowMessage.Invoke(message, isError, timeInMs);
		}
	}
}
