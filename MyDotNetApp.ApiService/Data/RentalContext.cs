using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Models;

namespace LuxuryRental.Api.Data
{
    public class RentalContext : DbContext
    {
        public RentalContext(DbContextOptions<RentalContext> options) : base(options) { }

        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Rental> Rentals => Set<Rental>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Car>().HasData(
                new Car { Id = 1, Make = "Ferrari", Model = "Roma", PricePerDay = 1200m, IsAvailable = true },
                new Car { Id = 2, Make = "Lamborghini", Model = "Huracán", PricePerDay = 1500m, IsAvailable = true }
            );
        }
    }
}