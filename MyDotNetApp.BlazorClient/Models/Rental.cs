namespace MyDotNetApp.BlazorClient.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car? Car { get; set; }
        public int UserId { get; set; }
        public string RenterName { get; set; } = "";
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public RentalStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum RentalStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}