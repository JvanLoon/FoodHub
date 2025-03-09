using FoodHub.Persistence.Entities;

using Microsoft.AspNetCore.Components;

namespace FoodCalc.Web.Components.Pages.Recepts;
public partial class CreateRecept
{
	[Inject]
	public IHttpClientFactory HttpClientFactory { get; set; }

	private Recept _recept = new();

    private async Task HandleValidSubmit()
    {
		var client = HttpClientFactory.CreateClient("FoodCalcApi");
		var response = await client.PostAsJsonAsync("api/recept", _recept);
        if (response.IsSuccessStatusCode)
        {
            // Handle success (e.g., navigate to a different page or show a success message)
        }
        else
        {
            // Handle error (e.g., show an error message)
        }
    }
}
