using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using LuxuryRental.Api.Models;
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

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var notifications = await _db.Notifications
                .Where(n => n.UserId == int.Parse(userId))
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var count = await _db.Notifications
                .Where(n => n.UserId == int.Parse(userId) && !n.IsRead)
                .CountAsync();

            return Ok(new { count });
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == int.Parse(userId));

            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _db.Notifications
                .Where(n => n.UserId == int.Parse(userId) && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

            return Ok();
        }
    }
}
