using System.ComponentModel.DataAnnotations;

namespace LuxuryRental.Api.Models
{
    public class Rental
    {
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }
        
        public Car? Car { get; set; }

        [Required]
        public int UserId { get; set; }
        
        public User? User { get; set; }

        [Required]
        public string RenterName { get; set; } = "";

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public decimal TotalPrice { get; set; }

        public RentalStatus Status { get; set; } = RentalStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}