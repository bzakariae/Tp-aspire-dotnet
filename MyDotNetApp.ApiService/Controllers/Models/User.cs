using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace MyDotNetApp.ApiService.Controllers.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public string Role { get; set; } = "Renter";

        public string KeycloakId { get; set; } = ""; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}