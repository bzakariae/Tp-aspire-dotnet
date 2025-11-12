namespace MyDotNetApp.BlazorClient.Services;

using System.Net.Http.Json;



using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;



public class KeycloakAuthService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;
    private readonly ILocalStorageService _storage;
    private readonly NavigationManager _nav;
    private const string Authority = "http://localhost:8080/realms/car-rental";

    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string IdTokenKey = "id_token";

    public KeycloakAuthService(
        HttpClient httpClient,
        CustomAuthStateProvider authStateProvider,
        ILocalStorageService storage,
        NavigationManager nav)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _storage = storage;
        _nav = nav;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new("client_id", "blazor-client"),
                new("username", username),
                new("password", password),
                new("grant_type", "password"),
                new("scope", "openid profile email roles")
            };

            var response = await _httpClient.PostAsync(
                $"{Authority}/protocol/openid-connect/token",
                new FormUrlEncodedContent(parameters));

            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
            if (content is null || string.IsNullOrWhiteSpace(content.access_token)) return false;

            // ✅ stocke tout
            await _storage.SetItemAsStringAsync(AccessTokenKey, content.access_token);
            if (!string.IsNullOrWhiteSpace(content.refresh_token))
                await _storage.SetItemAsStringAsync(RefreshTokenKey, content.refresh_token);
            if (!string.IsNullOrWhiteSpace(content.id_token))
                await _storage.SetItemAsStringAsync(IdTokenKey, content.id_token);

            await _authStateProvider.MarkUserAsAuthenticatedAsync(content.access_token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        // 1) vide le stockage + notifie
        var idToken = await _storage.GetItemAsStringAsync(IdTokenKey);
        await _storage.RemoveItemAsync(AccessTokenKey);
        await _storage.RemoveItemAsync(RefreshTokenKey);
        await _storage.RemoveItemAsync(IdTokenKey);
        await _authStateProvider.MarkUserAsLoggedOutAsync();

        // 2) logout SSO côté Keycloak + redirection
        var postLogout = _nav.BaseUri.TrimEnd('/');
        var url = $"{Authority}/protocol/openid-connect/logout" +
                  (string.IsNullOrEmpty(idToken) ? "" : $"?id_token_hint={Uri.EscapeDataString(idToken)}") +
                  $"{(string.IsNullOrEmpty(idToken) ? "?" : "&")}post_logout_redirect_uri={Uri.EscapeDataString(postLogout)}";

        _nav.NavigateTo(url, forceLoad: true);
    }
}


public class KeycloakTokenResponse
{
    public string access_token { get; set; } = string.Empty;
    public string refresh_token { get; set; } = string.Empty;
    public string id_token { get; set; } = string.Empty;
    public string token_type { get; set; } = string.Empty;
    public int expires_in { get; set; }
}