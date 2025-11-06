using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using MyDotNetApp.BlazorClient.Models;
using Microsoft.AspNetCore.Components;

namespace MyDotNetApp.BlazorClient.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigation;
        private const string TOKEN_KEY = "authToken";
        private const string USER_KEY = "authUser";
        private AuthConfig? _authConfig;

        public AuthService(HttpClient http, ILocalStorageService localStorage, NavigationManager navigation)
        {
            _http = http;
            _localStorage = localStorage;
            _navigation = navigation;
        }

        private async Task<AuthConfig?> GetAuthConfig()
        {
            if (_authConfig != null)
                return _authConfig;

            try
            {
                _authConfig = await _http.GetFromJsonAsync<AuthConfig>("api/auth/config");
                return _authConfig;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error getting auth config: {ex.Message}");
                return null;
            }
        }

        // ----- Discord OAuth pour utilisateurs normaux -----
        public async Task<string> InitiateDiscordLogin()
        {
            var config = await GetAuthConfig();
            if (config == null)
            {
                throw new Exception("Configuration Discord OAuth non disponible");
            }

            var redirectUri = $"{_navigation.BaseUri}auth/callback";
            var state = Guid.NewGuid().ToString();

            // Stocker state pour validation
            await _localStorage.SetItemAsync("oauth_state", state);

            var authUrl = $"{config.AuthorizationEndpoint}" +
                         $"?client_id={config.ClientId}" +
                         $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                         $"&response_type=code" +
                         $"&scope=identify email" +
                         $"&state={state}";

            Console.WriteLine($" Discord auth URL: {authUrl}");
            return authUrl;
        }

        public async Task<AuthResponse?> HandleCallback(string code, string state)
        {
            try
            {
                var storedState = await _localStorage.GetItemAsync<string>("oauth_state");
                if (storedState != state)
                {
                    Console.WriteLine(" Invalid OAuth state");
                    return null;
                }

                var redirectUri = $"{_navigation.BaseUri}auth/callback";

                var tokenRequest = new TokenExchangeRequest
                {
                    Code = code,
                    RedirectUri = redirectUri
                };

                var response = await _http.PostAsJsonAsync("api/auth/token", tokenRequest);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (authResponse != null)
                    {
                        await StoreAuthData(authResponse);
                        Console.WriteLine(" Discord login successful");
                        return authResponse;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($" Token exchange failed: {response.StatusCode} - {errorContent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Callback error: {ex.Message}");
            }
            finally
            {
                await _localStorage.RemoveItemAsync("oauth_state");
            }

            return null;
        }

        // ----- Login Admin via email + mot de passe -----
        public async Task<AuthResponse?> Login(LoginRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (authResponse != null)
                    {
                        await StoreAuthData(authResponse);
                        return authResponse;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        private async Task StoreAuthData(AuthResponse authResponse)
        {
            await _localStorage.SetItemAsync(TOKEN_KEY, authResponse.Token);
            await _localStorage.SetItemAsync(USER_KEY, authResponse);

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authResponse.Token);
        }

        public async Task Logout()
        {
            try
            {
                await _localStorage.RemoveItemAsync(TOKEN_KEY);
                await _localStorage.RemoveItemAsync(USER_KEY);
                _http.DefaultRequestHeaders.Authorization = null;
                Console.WriteLine(" Logged out");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Logout error: {ex.Message}");
            }
        }

        public async Task InitializeAuth()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
                if (!string.IsNullOrEmpty(token))
                {
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine(" Auth initialized with stored token");
                }
                else
                {
                    Console.WriteLine(" No stored token found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" InitializeAuth error: {ex.Message}");
            }
        }

        public async Task<bool> IsAuthenticated()
        {
            var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetRole()
        {
            var user = await _localStorage.GetItemAsync<AuthResponse>(USER_KEY);
            return user?.Role;
        }

        public async Task<string?> GetUserName()
        {
            var user = await _localStorage.GetItemAsync<AuthResponse>(USER_KEY);
            return user?.FullName ?? user?.Email;
        }

        public async Task<AuthResponse?> GetCurrentUser()
        {
            return await _localStorage.GetItemAsync<AuthResponse>(USER_KEY);
        }
    }

    // ----- Modèles -----
    public class AuthConfig
    {
        public string ClientId { get; set; } = "";
        public string AuthorizationEndpoint { get; set; } = "";
        public string TokenEndpoint { get; set; } = "";
        public string UserInfoEndpoint { get; set; } = "";
    }

    public class TokenExchangeRequest
    {
        public string Code { get; set; } = "";
        public string RedirectUri { get; set; } = "";
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "";
        public int UserId { get; set; }
    }
}
