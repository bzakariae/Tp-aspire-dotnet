using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using LuxuryRental.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyDotNetApp.ApiService.Controllers.Renter
{
    [ApiController]
    [Route("api/renter")]
    [Authorize(Roles = "Renter")]
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
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            var car = await _db.Cars.FindAsync(request.CarId);
            if (car == null || !car.IsAvailable) 
                return BadRequest(new { message = "Voiture non disponible" });

            var days = Math.Max(1, (request.EndDate.ToDateTime(new TimeOnly(0,0)) - request.StartDate.ToDateTime(new TimeOnly(0,0))).Days + 1);
            
            request.UserId = int.Parse(userId ?? "0");
            request.RenterName = userName ?? userEmail;
            request.TotalPrice = car.PricePerDay * days;
            request.CreatedAt = DateTime.UtcNow;

            car.IsAvailable = false;

            _db.Rentals.Add(request);
            await _db.SaveChangesAsync();

            var rental = await _db.Rentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == request.Id);

            return CreatedAtAction(nameof(GetRental), new { id = request.Id }, rental);
        }

        [HttpGet("rentals/{id:int}")]
        public async Task<IActionResult> GetRental(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var r = await _db.Rentals.Include(x => x.Car).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        [HttpGet("myrentals")]
        public async Task<IActionResult> MyRentals()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var list = await _db.Rentals
                .AsNoTracking()
                .Where(r => r.UserId == int.Parse(userId))
                .Include(r => r.Car)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return Ok(list);
        }
    }
}
