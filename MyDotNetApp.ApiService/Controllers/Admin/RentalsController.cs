using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using Microsoft.AspNetCore.Authorization;

namespace MyDotNetApp.ApiService.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/rentals")]
    [Authorize(Roles = "Admin")]
    public class AdminRentalsController : ControllerBase
    {
        private readonly RentalContext _db;

        public AdminRentalsController(RentalContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAllRentals()
        {
            var rentals = await _db.Rentals
                .Include(r => r.Car)
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return Ok(rentals);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalRentals = await _db.Rentals.CountAsync();
            var totalRevenue = await _db.Rentals.SumAsync(r => r.TotalPrice);
            var availableCars = await _db.Cars.CountAsync(c => c.IsAvailable);
            var totalCars = await _db.Cars.CountAsync();

            return Ok(new
            {
                totalRentals,
                totalRevenue,
                availableCars,
                totalCars
            });
        }
    }
}