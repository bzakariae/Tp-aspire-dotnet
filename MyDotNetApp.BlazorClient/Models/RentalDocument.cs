namespace MyDotNetApp.BlazorClient.Models
{
    public class RentalDocument
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public string DocumentType { get; set; } = "";
        public string FileName { get; set; } = "";
        public string? FileContent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}