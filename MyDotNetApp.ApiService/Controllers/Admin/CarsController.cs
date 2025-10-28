using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using LuxuryRental.Api.Models;

namespace MyDotNetApp.ApiService.Controllers.Admin
{
    [ApiController]
    [Route("admin/cars")]
    // TODO: ajouter [Authorize(Roles = "Admin")] et configurer l'authentification
    public class CarsController : ControllerBase
    {
        private readonly RentalContext _db;

        public CarsController(RentalContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Cars.AsNoTracking().ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var car = await _db.Cars.FindAsync(id);
            if (car == null) return NotFound();
            return Ok(car);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Car car)
        {
            _db.Cars.Add(car);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = car.Id }, car);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Car updated)
        {
            var car = await _db.Cars.FindAsync(id);
            if (car == null) return NotFound();
            car.Make = updated.Make;
            car.Model = updated.Model;
            car.PricePerDay = updated.PricePerDay;
            car.IsAvailable = updated.IsAvailable;
            car.ImageUrl = updated.ImageUrl;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _db.Cars.FindAsync(id);
            if (car == null) return NotFound();
            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}