using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FoodHub.Persistence.Entities
{
    public class User : IdentityUser
    {
        public bool Enabled { get; set; } = false;
        // Additional custom properties can be added here if needed
    }
}
