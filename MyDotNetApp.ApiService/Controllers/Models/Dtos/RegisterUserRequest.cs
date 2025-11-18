namespace MyDotNetApp.ApiService.Controllers.Models.Dtos;

public class RegisterUserRequest
{
    public string Email { get; set; } = "";
    public string FirstName{ get; set; } = "";
    public string LastName{ get; set; } = "";
    public string Password { get; set; } = "";
}