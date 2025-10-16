namespace MyDotNetApp.ApiService.Data;
using Microsoft.EntityFrameworkCore;
using MyDotNetApp.ApiService.Models;

public class TicketContext : DbContext
{
    public TicketContext(DbContextOptions<TicketContext> options) : base(options) { }

    public DbSet<Ticket> Tickets { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data
        modelBuilder.Entity<Ticket>().HasData(
            new Ticket { Id = 1, Title = "Test Ticket 1", Description = "This is a test ticket" }
        );
    }
}