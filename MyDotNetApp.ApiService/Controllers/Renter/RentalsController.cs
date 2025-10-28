using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using LuxuryRental.Api.Models;

namespace MyDotNetApp.ApiService.Controllers.Renter
{
    [ApiController]
    [Route("renter")]
    public class RentalsController : ControllerBase
    {
        private readonly RentalContext _db;

        public RentalsController(RentalContext db) => _db = db;

        [HttpGet("cars")]
        public async Task<IActionResult> AvailableCars()
        {
            var cars = await _db.Cars.AsNoTracking().Where(c => c.IsAvailable).ToListAsync();
            return Ok(cars);
        }

        [HttpPost("rent")]
        public async Task<IActionResult> Rent([FromBody] Rental request)
        {
            var car = await _db.Cars.FindAsync(request.CarId);
            if (car == null || !car.IsAvailable) return BadRequest("Voiture non disponible");

            var days = Math.Max(1, (request.EndDate.ToDateTime(new TimeOnly(0,0)) - request.StartDate.ToDateTime(new TimeOnly(0,0))).Days + 1);
            request.TotalPrice = car.PricePerDay * days;
            request.CreatedAt = DateTime.UtcNow;

            car.IsAvailable = false;

            _db.Rentals.Add(request);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRental), new { id = request.Id }, request);
        }

        [HttpGet("rentals/{id:int}")]
        public async Task<IActionResult> GetRental(int id)
        {
            var r = await _db.Rentals.Include(x => x.Car).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        [HttpGet("myrentals")]
        public async Task<IActionResult> MyRentals([FromQuery] string renterName)
        {
            var list = await _db.Rentals.AsNoTracking().Where(r => r.RenterName == renterName).Include(r => r.Car).ToListAsync();
            return Ok(list);
        }
    }
}