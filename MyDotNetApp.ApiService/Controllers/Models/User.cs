using System.ComponentModel.DataAnnotations;

namespace LuxuryRental.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public string Role { get; set; } = "Renter"; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}