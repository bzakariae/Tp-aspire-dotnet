namespace MyDotNetApp.BlazorClient.Models
{
    public class RentalRequest
    {
        public int CarId { get; set; }
        public int UserId { get; set; }
        public string RenterName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}