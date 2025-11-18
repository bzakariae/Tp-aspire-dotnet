namespace MyDotNetApp.ApiService.Services;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class KeycloakAdminService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public KeycloakAdminService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<string> CreateUserAsync(string email, string firstName,string lastName, string password)
    {
        var baseUrl = _config["Keycloak:BaseUrl"];   
        var realm   = _config["Keycloak:Realm"];     

        var token = await GetAdminTokenAsync();

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            username = email,
            email = email,
            emailVerified = true,  
            enabled = true,
            firstName = firstName,
            lastName = lastName,
        };

        var resp = await _http.PostAsync(
            $"{baseUrl}/admin/realms/{realm}/users",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

        resp.EnsureSuccessStatusCode();

        var location = resp.Headers.Location!.ToString();
        var userId = location.Split('/').Last();

        var passPayload = new
        {
            type = "password",
            value = password,
            temporary = false
        };

        var passResp = await _http.PutAsync(
            $"{baseUrl}/admin/realms/{realm}/users/{userId}/reset-password",
            new StringContent(JsonSerializer.Serialize(passPayload), Encoding.UTF8, "application/json"));

        passResp.EnsureSuccessStatusCode();
        await AssignRealmRoleAsync(userId, "ROLE_RENTAL_MANAGER");
        return userId;
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var baseUrl = _config["Keycloak:BaseUrl"];
        var realm   = _config["Keycloak:Realm"];
        var clientId     = _config["Keycloak:AdminClientId"];
        var clientSecret = _config["Keycloak:AdminClientSecret"];

        var data = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = clientId!,
            ["client_secret"] = clientSecret!
        };

        var resp = await _http.PostAsync(
            $"{baseUrl}/realms/{realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(data));

        resp.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("access_token").GetString()!;
    }
    public async Task AssignRealmRoleAsync(string userId, string roleName)
    {
        var baseUrl = _config["Keycloak:BaseUrl"];
        var realm   = _config["Keycloak:Realm"];
        var token   = await GetAdminTokenAsync();

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // 1. Récupérer le rôle tel quel
        var roleResp = await _http.GetAsync(
            $"{baseUrl}/admin/realms/{realm}/roles/{roleName}");
        roleResp.EnsureSuccessStatusCode();

        var roleJson = await roleResp.Content.ReadAsStringAsync();

        // 2. Le renvoyer directement dans un tableau JSON
        var body = "[" + roleJson + "]";

        var assignResp = await _http.PostAsync(
            $"{baseUrl}/admin/realms/{realm}/users/{userId}/role-mappings/realm",
            new StringContent(body, Encoding.UTF8, "application/json"));

        assignResp.EnsureSuccessStatusCode();
    }


   

}
