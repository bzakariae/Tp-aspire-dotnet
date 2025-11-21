namespace MyDotNetApp.ApiService.Controllers.Admin;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyDotNetApp.ApiService.Data;
using MyDotNetApp.ApiService.Services;
using MyDotNetApp.ApiService.Controllers.Models.Dtos;
using MyDotNetApp.ApiService.Controllers.Models;


[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly RentalContext _db;
    private readonly KeycloakAdminService _kc;

    public AdminController(RentalContext db, KeycloakAdminService kc)
    {
        _db = db;
        _kc = kc;
    }

    [HttpPost("create")]
    [Authorize(Roles = "ROLE_RENTAL_MANAGER")]
    public async Task<IActionResult> CreateAdmin([FromBody] RegisterUserRequest req)
    {
        // 1) Créer l’utilisateur dans Keycloak avec le role admin
        var keycloakId = await _kc.CreateAdminAsync(req.Email, req.FirstName, req.LastName, req.Password);

        // 2) Sauvegarder dans la DB
        var user = new User
        {
            Email = req.Email,
            FullName = $"{req.FirstName} {req.LastName}",
            Role = "Admin",
            KeycloakId = keycloakId
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(user);
    }
}
