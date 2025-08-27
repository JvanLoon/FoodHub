using System.ComponentModel.DataAnnotations;

namespace FoodHub.Persistence.Entities
{
    using Microsoft.AspNetCore.Identity;
    public class User : IdentityUser
    {
        public bool Enabled { get; set; } = false;
        // Additional custom properties can be added here if needed
    }
}
