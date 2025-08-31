using Microsoft.AspNetCore.Identity;

namespace FoodHub.Persistence.Entities
{
    public class User : IdentityUser
    {
        public bool Enabled { get; set; } = false;
		public List<string> Roles { get; set; }
	}
}
