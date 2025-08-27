using System.ComponentModel.DataAnnotations;

namespace FoodHub.Persistence.Entities
{
    public class User : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        // Add more fields as needed (Email, etc.)
    }
}
