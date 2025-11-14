using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDotNetApp.ApiService.Data;
using MyDotNetApp.ApiService.Controllers.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyDotNetApp.ApiService.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly RentalContext _db;

        public NotificationsController(RentalContext db) => _db = db;

        private async Task<User?> GetCurrentUserAsync()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value 
                        ?? User.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(email))
                return null;

            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound(new { message = "Utilisateur inexistant dans la base de données." });

            var notifications = await _db.Notifications
                .Where(n => n.UserId == user.Id)
                .Include(n => n.RelatedRental)
                    .ThenInclude(r => r!.Car)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound(new { message = "Utilisateur inexistant dans la base de données." });

            var count = await _db.Notifications
                .Where(n => n.UserId == user.Id && !n.IsRead)
                .CountAsync();

            return Ok(new { count });
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound(new { message = "Utilisateur inexistant dans la base de données." });

            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == user.Id);

            if (notification == null)
                return NotFound(new { message = "Notification introuvable pour cet utilisateur." });

            notification.IsRead = true;
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound(new { message = "Utilisateur inexistant dans la base de données." });

            await _db.Notifications
                .Where(n => n.UserId == user.Id && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

            return Ok();
        }
    }
}
