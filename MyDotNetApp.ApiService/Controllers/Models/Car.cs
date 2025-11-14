using System.ComponentModel.DataAnnotations;

namespace MyDotNetApp.ApiService.Controllers.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string Make { get; set; } = "";

        [Required]
        public string Model { get; set; } = "";

        public string Class { get; set; } = "Luxury";

        public decimal PricePerDay { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string? ImageUrl { get; set; }
    }
}