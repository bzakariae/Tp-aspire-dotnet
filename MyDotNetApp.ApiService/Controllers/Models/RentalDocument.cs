using System.ComponentModel.DataAnnotations;

namespace MyDotNetApp.ApiService.Controllers.Models
{
    public class RentalDocument
    {
        public int Id { get; set; }

        [Required]
        public int RentalId { get; set; }
        
        public Rental? Rental { get; set; }

        [Required]
        public string DocumentType { get; set; } = ""; 

        [Required]
        public string FileName { get; set; } = "";

        public string? FileContent { get; set; } // Base64 encoded PDF content

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}