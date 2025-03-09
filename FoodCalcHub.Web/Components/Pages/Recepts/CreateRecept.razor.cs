using FoodHub.Persistence.Entities;

namespace FoodCalc.Web.Components.Pages.Recepts;
public partial class CreateRecept
{
    private Recept _recept;

    private async Task HandleValidSubmit()
    {
        var response = await Http.PostAsJsonAsync("api/recepts", _recept);
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
