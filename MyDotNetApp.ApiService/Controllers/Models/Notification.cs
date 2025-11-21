using System.ComponentModel.DataAnnotations;

namespace MyDotNetApp.ApiService.Controllers.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public User? User { get; set; }

        [Required]
        public string Type { get; set; } = ""; // "RentalRequest", "RentalApproved", "RentalRejected"

        [Required]
        public string Message { get; set; } = "";

        public bool IsRead { get; set; } = false;

        public int? RelatedRentalId { get; set; }

        public Rental? RelatedRental { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}