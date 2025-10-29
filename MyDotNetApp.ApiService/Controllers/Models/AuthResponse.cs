namespace LuxuryRental.Api.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "";
        public int UserId { get; set; }
    }
}