using Microsoft.EntityFrameworkCore;

namespace MyDotNetApp.ApiService.Data
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public bool Done { get; set; }
    }

    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options) : base(options) {}
        public DbSet<Ticket> Tickets => Set<Ticket>();
    }
}