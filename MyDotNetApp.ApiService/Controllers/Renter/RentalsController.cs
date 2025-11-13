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
    [Authorize(Roles = "ROLE_RENTAL_CUSTOMER")]
    public class RentalsController : ControllerBase
    {
        private readonly RentalContext _db;
        private readonly ILogger<RentalsController> _logger;

        public RentalsController(RentalContext db, ILogger<RentalsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // 🔹 Utilitaire : récupérer l'utilisateur courant via l'EMAIL du token Keycloak
        private async Task<User?> GetCurrentUserAsync()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("GetCurrentUserAsync: email claim not found.");
                return null;
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("GetCurrentUserAsync: user not found for email {Email}", email);
            }

            return user;
        }

        [HttpGet("cars")]
        public async Task<IActionResult> AvailableCars()
        {
            var cars = await _db.Cars
                .AsNoTracking()
                .Where(c => c.IsAvailable)
                .ToListAsync();

            return Ok(cars);
        }

        [HttpPost("rent")]
        public async Task<IActionResult> Rent([FromBody] Rental request)
        {
            try
            {
                _logger.LogInformation("Rent: Starting rental creation");

                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    _logger.LogError("Rent: User not found from claims");
                    return BadRequest(new { message = "Utilisateur introuvable" });
                }

                _logger.LogInformation("Rent: Found user ID={UserId}, Email={Email}", user.Id, user.Email);

                var car = await _db.Cars.FindAsync(request.CarId);
                if (car == null)
                {
                    _logger.LogWarning("Rent: Car not found, CarId={CarId}", request.CarId);
                    return BadRequest(new { message = "Voiture introuvable" });
                }

                if (!car.IsAvailable)
                {
                    _logger.LogWarning("Rent: Car not available, CarId={CarId}", request.CarId);
                    return BadRequest(new { message = "Voiture non disponible" });
                }

                var days = (request.EndDate.ToDateTime(new TimeOnly(0, 0)) -
                            request.StartDate.ToDateTime(new TimeOnly(0, 0))).Days;

                if (days <= 0)
                {
                    _logger.LogWarning("Rent: Invalid date range, Start={Start}, End={End}",
                        request.StartDate, request.EndDate);
                    return BadRequest(new { message = "La date de fin doit être après la date de début" });
                }

                _logger.LogInformation("Rent: Days={Days}, PricePerDay={PricePerDay}", days, car.PricePerDay);

                request.UserId = user.Id;
                request.RenterName = user.FullName ?? user.Email;
                request.TotalPrice = car.PricePerDay * days;
                request.CreatedAt = DateTime.UtcNow;
                request.Status = RentalStatus.Pending;

                _logger.LogInformation("Rent: Creating rental - UserId={UserId}, TotalPrice={TotalPrice}, Status={Status}",
                    request.UserId, request.TotalPrice, request.Status);

                car.IsAvailable = false;

                _db.Rentals.Add(request);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Rent: Rental created with ID={RentalId}", request.Id);

                var adminUsers = await _db.Users
                    .Where(u => u.Role == "Admin")
                    .ToListAsync();

                _logger.LogInformation("Rent: Found {AdminCount} admin users for notifications", adminUsers.Count);

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

                _logger.LogInformation("Rent: Notifications created successfully");

                var rental = await _db.Rentals
                    .Include(r => r.Car)
                    .FirstOrDefaultAsync(r => r.Id == request.Id);

                _logger.LogInformation("Rent: Rental creation completed successfully");

                return CreatedAtAction(nameof(GetRental), new { id = request.Id }, rental);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rent: Exception occurred");
                return StatusCode(500, new
                {
                    message = "Une erreur est survenue lors de la création de la réservation",
                    error = ex.Message
                });
            }
        }

        [HttpGet("rentals/{id:int}")]
        public async Task<IActionResult> GetRental(int id)
        {
            // Ici on ne filtre pas par user pour l’instant (comme dans ton code original),
            // mais on pourrait, si tu veux, vérifier que le rental appartient bien à l’utilisateur.
            var rental = await _db.Rentals
                .Include(x => x.Car)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (rental == null)
                return NotFound();

            return Ok(rental);
        }

        [HttpGet("myrentals")]
        public async Task<IActionResult> MyRentals()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound(new { message = "Utilisateur inexistant dans la base de données." });

            var list = await _db.Rentals
                .AsNoTracking()
                .Where(r => r.UserId == user.Id)   // ✅ plus de int.Parse sur NameIdentifier
                .Include(r => r.Car)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("rentals/{id}/documents")]
        public async Task<IActionResult> GetRentalDocuments(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound(new { message = "Utilisateur inexistant dans la base de données." });

            var rental = await _db.Rentals
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == user.Id);

            if (rental == null)
                return NotFound(new { message = "Réservation introuvable pour cet utilisateur." });

            var documents = await _db.RentalDocuments
                .Where(d => d.RentalId == id)
                .ToListAsync();

            return Ok(documents);
        }
    }
}
