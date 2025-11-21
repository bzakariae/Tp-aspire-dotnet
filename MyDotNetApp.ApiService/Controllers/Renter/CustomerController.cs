namespace MyDotNetApp.ApiService.Controllers.Renter;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyDotNetApp.ApiService.Data;
using MyDotNetApp.ApiService.Services;
using MyDotNetApp.ApiService.Controllers.Models;
using MyDotNetApp.ApiService.Controllers.Models.Dtos;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly RentalContext _db;
    private readonly KeycloakAdminService _kc;

    public CustomerController(RentalContext db, KeycloakAdminService kc)
    {
        _db = db;
        _kc = kc;
    }


    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterUserRequest req)
    {

        var exists = await _db.Users.AnyAsync(u => u.Email == req.Email);
        if (exists)
        {
            return BadRequest("Cet email est déjà utilisé.");
        }

        try
        {
            var keycloakId = await _kc.CreateCustomerAsync(
                req.Email,
                req.FirstName,
                req.LastName,
                req.Password
            );


            var user = new User
            {
                Email = req.Email,
                FullName = $"{req.FirstName} {req.LastName}",
                Role = "Client", // ou "ROLE_RENTAL_CUSTOMER" si tu veux aligner
                KeycloakId = keycloakId
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(user);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Keycloak"))
        {
            return BadRequest("Cet email est déjà utilisé (Keycloak).");
        }
    }
}