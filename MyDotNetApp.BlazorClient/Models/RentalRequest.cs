namespace MyDotNetApp.BlazorClient.Models
{
    public class RentalRequest
    {
        public int CarId { get; set; }
        public string RenterName { get; set; } = "";
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}