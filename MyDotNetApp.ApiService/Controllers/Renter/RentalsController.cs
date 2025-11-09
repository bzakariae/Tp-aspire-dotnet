using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using LuxuryRental.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace MyDotNetApp.ApiService.Controllers.Renter
{
    [ApiController]
    [Route("api/renter")]
    [Authorize(Roles = "Renter")]
    public class RentalsController : ControllerBase
    {
        private readonly RentalContext _db;
        private readonly ILogger<RentalsController> _logger;

        public RentalsController(RentalContext db, ILogger<RentalsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("cars")]
        public async Task<IActionResult> AvailableCars()
        {
            var cars = await _db.Cars.AsNoTracking().Where(c => c.IsAvailable).ToListAsync();
            return Ok(cars);
        }

        [HttpPost("rent")]
        public async Task<IActionResult> Rent([FromBody] Rental request)
        {
            try
            {
                _logger.LogInformation(" Rent: Starting rental creation");
                
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                _logger.LogInformation($" Rent: UserEmail={userEmail}, UserName={userName}, UserIdClaim={userIdClaim}");

                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning(" Rent: User not authenticated");
                    return Unauthorized(new { message = "Utilisateur non authentifié" });
                }

                User? user = null;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    user = await _db.Users.FindAsync(userId);
                }
                
                if (user == null)
                {
                    user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                }

                if (user == null)
                {
                    _logger.LogError($" Rent: User not found for email {userEmail}");
                    return BadRequest(new { message = "Utilisateur introuvable" });
                }

                _logger.LogInformation($" Rent: Found user ID={user.Id}, Email={user.Email}");

                var car = await _db.Cars.FindAsync(request.CarId);
                if (car == null)
                {
                    _logger.LogWarning($" Rent: Car not found, CarId={request.CarId}");
                    return BadRequest(new { message = "Voiture introuvable" });
                }

                if (!car.IsAvailable)
                {
                    _logger.LogWarning($" Rent: Car not available, CarId={request.CarId}");
                    return BadRequest(new { message = "Voiture non disponible" });
                }

                var days = (request.EndDate.ToDateTime(new TimeOnly(0,0)) - request.StartDate.ToDateTime(new TimeOnly(0,0))).Days;
                if (days <= 0)
                {
                    _logger.LogWarning($" Rent: Invalid date range, Start={request.StartDate}, End={request.EndDate}");
                    return BadRequest(new { message = "La date de fin doit être après la date de début" });
                }

                _logger.LogInformation($" Rent: Days={days}, PricePerDay={car.PricePerDay}");

                request.UserId = user.Id;
                request.RenterName = user.FullName ?? user.Email;
                request.TotalPrice = car.PricePerDay * days;
                request.CreatedAt = DateTime.UtcNow;
                request.Status = RentalStatus.Pending;

                _logger.LogInformation($" Rent: Creating rental - UserId={request.UserId}, TotalPrice={request.TotalPrice}, Status={request.Status}");

                car.IsAvailable = false;

                _db.Rentals.Add(request);
                await _db.SaveChangesAsync();

                _logger.LogInformation($" Rent: Rental created with ID={request.Id}");

                var adminUsers = await _db.Users.Where(u => u.Role == "Admin").ToListAsync();
                _logger.LogInformation($" Rent: Found {adminUsers.Count} admin users for notifications");

                foreach (var admin in adminUsers)
                {
                    var notification = new Notification
                    {
                        UserId = admin.Id,
                        Type = "RentalRequest",
                        Message = $"Nouvelle demande de réservation de {user.FullName ?? user.Email} pour {car.Make} {car.Model}",
                        RelatedRentalId = request.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    };
                    _db.Notifications.Add(notification);
                }

                await _db.SaveChangesAsync();

                _logger.LogInformation($" Rent: Notifications created successfully");

                var rental = await _db.Rentals
                    .Include(r => r.Car)
                    .FirstOrDefaultAsync(r => r.Id == request.Id);

                _logger.LogInformation($" Rent: Rental creation completed successfully");

                return CreatedAtAction(nameof(GetRental), new { id = request.Id }, rental);
            }
            catch (Exception ex)
            {
                _logger.LogError($" Rent: Exception occurred - {ex.Message}");
                _logger.LogError($" Rent: Stack trace - {ex.StackTrace}");
                return StatusCode(500, new { message = "Une erreur est survenue lors de la création de la réservation", error = ex.Message });
            }
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

        [HttpGet("rentals/{id}/documents")]
        public async Task<IActionResult> GetRentalDocuments(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var rental = await _db.Rentals
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == int.Parse(userId));

            if (rental == null)
                return NotFound();

            var documents = await _db.RentalDocuments
                .Where(d => d.RentalId == id)
                .ToListAsync();

            return Ok(documents);
        }
    }
}
