using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Models;

namespace LuxuryRental.Api.Data
{
    public class RentalContext : DbContext
    {
        public RentalContext(DbContextOptions<RentalContext> options) : base(options) { }

        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Rental> Rentals => Set<Rental>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Rental>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasOne<Car>()
                .WithMany()
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Car>().HasData(
                new Car 
                { 
                    Id = 1, 
                    Make = "Ferrari", 
                    Model = "Roma", 
                    Class = "Supercar",
                    PricePerDay = 1200m, 
                    IsAvailable = true, 
                    ImageUrl = "https://images.unsplash.com/photo-1583121274602-3e2820c69888?w=800&h=600&fit=crop"
                },
                new Car 
                { 
                    Id = 2, 
                    Make = "Lamborghini", 
                    Model = "Huracán", 
                    Class = "Supercar",
                    PricePerDay = 1500m, 
                    IsAvailable = true, 
                    ImageUrl = "https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=800&h=600&fit=crop"
                },
                new Car 
                { 
                    Id = 3, 
                    Make = "Porsche", 
                    Model = "911 Turbo S", 
                    Class = "Sports",
                    PricePerDay = 900m, 
                    IsAvailable = true, 
                    ImageUrl = "https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=800&h=600&fit=crop"
                },
                new Car 
                { 
                    Id = 4, 
                    Make = "McLaren", 
                    Model = "720S", 
                    Class = "Supercar",
                    PricePerDay = 1300m, 
                    IsAvailable = true, 
                    ImageUrl = "https://images.unsplash.com/photo-1568605117036-5fe5e7bab0b7?w=800&h=600&fit=crop"
                },
                new Car 
                { 
                    Id = 5, 
                    Make = "Bentley", 
                    Model = "Continental GT", 
                    Class = "Luxury",
                    PricePerDay = 800m, 
                    IsAvailable = true, 
                    ImageUrl = "https://images.unsplash.com/photo-15493995427e3f8b79c341?w=800&h=600&fit=crop"
                },
                new Car 
                { 
                    Id = 6, 
                    Make = "Aston Martin", 
                    Model = "DB11", 
                    Class = "Grand Tourer",
                    PricePerDay = 1100m, 
                    IsAvailable = true, 
                    ImageUrl = "https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=800&h=600&fit=crop"
                },
                new Car 
                { 
                    Id = 7, 
                    Make = "Rolls-Royce", 
                    Model = "Wraith", 
                    Class = "Ultra Luxury",
                    PricePerDay = 1800m, 
                    IsAvailable = true, 
                    ImageUrl = "https://images.unsplash.com/photo-1563720360172-67b8f3dce741?w=800&h=600&fit=crop"
                },
                new Car 
                { 
                    Id = 8, 
                    Make = "Mercedes-AMG", 
                    Model = "GT R", 
                    Class = "Sports",
                    PricePerDay = 950m, 
                    IsAvailable = true, 
                    ImageUrl = "https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=800&h=600&fit=crop"
                }
            );
        }
    }
}
