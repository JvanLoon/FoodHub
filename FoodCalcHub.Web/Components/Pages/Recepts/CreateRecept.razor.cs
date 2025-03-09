using FoodHub.Persistence.Entities;

using Microsoft.AspNetCore.Components;

namespace FoodCalc.Web.Components.Pages.Recepts;
public partial class CreateRecept
{
	[Inject]
	public IHttpClientFactory HttpClientFactory { get; set; }

	private Recept _recept;
	private string _errorMessage;

	private async Task HandleValidSubmit()
	{
		try
		{
			var client = HttpClientFactory.CreateClient("FoodCalcApi");
			var response = await client.PostAsJsonAsync("api/recept", _recept);
			if (!response.IsSuccessStatusCode)
			{
				_errorMessage = "Something went wrong: " + response.Content + " " + response.StatusCode;
			}
			else
			{
				_errorMessage = null;
				// Handle successful submission (e.g., navigate to another page)
			}
		}
		catch (Exception ex)
		{
			_errorMessage = $"An unexpected error occurred: {ex.Message}";
		}
	}
}
