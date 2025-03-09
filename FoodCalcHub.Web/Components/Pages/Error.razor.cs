using Microsoft.AspNetCore.Components;

using System.Diagnostics;

namespace FoodCalc.Web.Components.Pages;
public partial class Error
{
	[CascadingParameter]
	public HttpContext? HttpContext { get; set; }

	private string? _requestId;
	private bool ShowRequestId => !string.IsNullOrEmpty(_requestId);

	protected override void OnInitialized()
	{
		_requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
	}
}
