using FoodHub.Persistence.Entities;

namespace FoodCalc.Web.Services
{
	public class ReceptService(HttpClient httpClient)
	{
		public async Task<HttpResponseMessage> SaveNewRecept(Recept recept)
		{
			return await httpClient.PostAsJsonAsync("api/recepts", recept);
		}
	}
}
