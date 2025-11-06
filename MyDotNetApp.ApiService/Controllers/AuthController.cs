using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using LuxuryRental.Api.Models;
using LuxuryRental.Api.Services;
using System.Text.Json;
using System.Net.Http.Headers;

namespace MyDotNetApp.ApiService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RentalContext _db;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AuthController(RentalContext db, JwtService jwtService, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _jwtService = jwtService;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("config")]
        public IActionResult GetAuthConfig()
        {
            var clientId = _configuration["Discord:ClientId"];
            
            if (string.IsNullOrEmpty(clientId))
            {
                return BadRequest(new { message = "Discord OAuth non configuré" });
            }

            return Ok(new
            {
                ClientId = clientId,
                AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize",
                TokenEndpoint = "https://discord.com/api/oauth2/token",
                UserInfoEndpoint = "https://discord.com/api/users/@me"
            });
        }

        [HttpPost("token")]
        public async Task<IActionResult> ExchangeToken([FromBody] TokenExchangeRequest request)
        {
            try
            {
                var clientId = _configuration["Discord:ClientId"];
                var clientSecret = _configuration["Discord:ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    return BadRequest(new { message = "Discord OAuth non configuré" });
                }

                // Échanger le code contre un access token
                var tokenRequest = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", request.Code),
                    new KeyValuePair<string, string>("redirect_uri", request.RedirectUri)
                });

                var tokenResponse = await _httpClient.PostAsync("https://discord.com/api/oauth2/token", tokenRequest);
                
                if (!tokenResponse.IsSuccessStatusCode)
                {
                    var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($" Discord token error: {errorContent}");
                    return BadRequest(new { message = "Échec de l'authentification Discord" });
                }

                var tokenData = await tokenResponse.Content.ReadFromJsonAsync<DiscordTokenResponse>();
                
                if (tokenData == null || string.IsNullOrEmpty(tokenData.AccessToken))
                {
                    return BadRequest(new { message = "Token Discord invalide" });
                }

                // Récupérer les informations utilisateur depuis Discord
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);
                var userResponse = await _httpClient.GetAsync("https://discord.com/api/users/@me");
                
                if (!userResponse.IsSuccessStatusCode)
                {
                    return BadRequest(new { message = "Impossible de récupérer les informations utilisateur" });
                }

                var discordUser = await userResponse.Content.ReadFromJsonAsync<DiscordUser>();
                
                if (discordUser == null)
                {
                    return BadRequest(new { message = "Données utilisateur invalides" });
                }

                // Créer ou mettre à jour l'utilisateur dans la base de données
                var email = discordUser.Email ?? $"{discordUser.Id}@discord.user";
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // Créer un nouvel utilisateur
                    user = new User
                    {
                        Email = email,
                        FullName = discordUser.Username,
                        PasswordHash = "", // Pas de mot de passe pour OAuth
                        Role = "Renter", // Par défaut, les nouveaux utilisateurs sont des locataires
                        CreatedAt = DateTime.UtcNow
                    };
                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // Mettre à jour le nom si nécessaire
                    if (user.FullName != discordUser.Username)
                    {
                        user.FullName = discordUser.Username;
                        await _db.SaveChangesAsync();
                    }
                }

                // Générer notre JWT interne
                var jwtToken = _jwtService.GenerateToken(user);

                return Ok(new AuthResponse
                {
                    Token = jwtToken,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    UserId = user.Id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Token exchange error: {ex.Message}");
                return StatusCode(500, new { message = "Erreur lors de l'authentification" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });

            // Gestion spéciale pour l’administrateur
            if (user.Role == "Admin" && user.Email == "admin@yassine.com")
            {
                if (request.Password != user.PasswordHash)
                    return Unauthorized(new { message = "Mot de passe administrateur incorrect" });
            }
            else
            {
                // Utilisateurs normaux (Discord ou enregistrés avec hash)
                if (string.IsNullOrEmpty(user.PasswordHash))
                    return Unauthorized(new { message = "Veuillez vous connecter avec Discord" });

                bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

                if (!passwordValid)
                    return Unauthorized(new { message = "Email ou mot de passe incorrect" });
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                UserId = user.Id
            });
        }
    }
    public class TokenExchangeRequest
    {
        public string Code { get; set; } = "";
        public string RedirectUri { get; set; } = "";
    }

    public class DiscordTokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("scope")]
        public string Scope { get; set; } = "";
    }

    public class DiscordUser
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("username")]
        public string Username { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("discriminator")]
        public string Discriminator { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("avatar")]
        public string? Avatar { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string? Email { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("verified")]
        public bool? Verified { get; set; }
    }
}
