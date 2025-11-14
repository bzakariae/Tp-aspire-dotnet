using System.ComponentModel.DataAnnotations;

namespace MyDotNetApp.ApiService.Controllers.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}