using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using LuxuryRental.Api.Models;
using Microsoft.AspNetCore.Authorization;
using MyDotNetApp.ApiService.Services;

namespace MyDotNetApp.ApiService.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/rentals")]
    [Authorize(Roles = "Admin")]
    public class AdminRentalsController : ControllerBase
    {
        private readonly RentalContext _db;
        private readonly DocumentService _documentService;

        public AdminRentalsController(RentalContext db, DocumentService documentService)
        {
            _db = db;
            _documentService = documentService;
        }

        [HttpGet("rentals/pending")]
        public async Task<IActionResult> GetPendingRentals()
        {
            var pendingRentals = await _db.Rentals
                .Where(r => r.Status == RentalStatus.Pending)
                .Include(r => r.Car)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(pendingRentals);
        }

        [HttpGet("rentals/approved")]
        public async Task<IActionResult> GetApprovedRentals()
        {
            var approvedRentals = await _db.Rentals
                .Where(r => r.Status == RentalStatus.Approved)
                .Include(r => r.Car)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(approvedRentals);
        }

        [HttpGet("rentals/all")]
        public async Task<IActionResult> GetAllRentals()
        {
            var allRentals = await _db.Rentals
                .Include(r => r.Car)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(allRentals);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveRental(int id)
        {
            var rental = await _db.Rentals
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
                return NotFound();

            if (rental.Status != RentalStatus.Pending)
                return BadRequest(new { message = "Cette réservation a déjà été traitée" });

            // Update rental status
            rental.Status = RentalStatus.Approved;

            // Generate documents
            var contract = new RentalDocument
            {
                RentalId = rental.Id,
                DocumentType = "Contract",
                FileName = $"Contrat_{rental.Id}.txt",
                FileContent = _documentService.GenerateContract(rental),
                CreatedAt = DateTime.UtcNow
            };

            var invoice = new RentalDocument
            {
                RentalId = rental.Id,
                DocumentType = "Invoice",
                FileName = $"Facture_{rental.Id}.txt",
                FileContent = _documentService.GenerateInvoice(rental),
                CreatedAt = DateTime.UtcNow
            };

            _db.RentalDocuments.AddRange(contract, invoice);

            // Create notification for user
            var notification = new Notification
            {
                UserId = rental.UserId,
                Type = "RentalApproved",
                Message = $"Votre réservation pour {rental.Car?.Make} {rental.Car?.Model} a été approuvée !",
                RelatedRentalId = rental.Id,
                CreatedAt = DateTime.UtcNow
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Réservation approuvée avec succès" });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = new
            {
                TotalRentals = await _db.Rentals.CountAsync(),
                TotalRevenue = await _db.Rentals
                    .Where(r => r.Status == RentalStatus.Approved)
                    .SumAsync(r => (decimal?)r.TotalPrice) ?? 0,
                AvailableCars = await _db.Cars.CountAsync(c => c.IsAvailable),
                TotalCars = await _db.Cars.CountAsync()
            };

            return Ok(stats);
        }
        
       
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectRental(int id, [FromBody] RejectRequest request)
        {
            var rental = await _db.Rentals
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
                return NotFound();

            if (rental.Status != RentalStatus.Pending)
                return BadRequest(new { message = "Cette réservation a déjà été traitée" });

            // Update rental status
            rental.Status = RentalStatus.Rejected;

            // Make car available again
            if (rental.Car != null)
            {
                rental.Car.IsAvailable = true;
            }

            // Create notification for user
            var notification = new Notification
            {
                UserId = rental.UserId,
                Type = "RentalRejected",
                Message = $"Votre réservation pour {rental.Car?.Make} {rental.Car?.Model} a été refusée. Raison: {request.Reason}",
                RelatedRentalId = rental.Id,
                CreatedAt = DateTime.UtcNow
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Réservation refusée" });
        }

        public class RejectRequest
        {
            public string Reason { get; set; } = "Non spécifiée";
        }
    }
}
