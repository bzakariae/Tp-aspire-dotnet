namespace MyDotNetApp.BlazorClient.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public Car? Car { get; set; }
        public string RenterName { get; set; } = "";
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}