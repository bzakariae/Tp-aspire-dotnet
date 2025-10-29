using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using MyDotNetApp.BlazorClient.Models;

namespace MyDotNetApp.BlazorClient.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private const string TOKEN_KEY = "authToken";
        private const string USER_KEY = "authUser";

        public AuthService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

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
                        // Stocker le token et les infos utilisateur
                        await _localStorage.SetItemAsync(TOKEN_KEY, authResponse.Token);
                        await _localStorage.SetItemAsync(USER_KEY, authResponse);
                        
                        // Ajouter le token aux headers HTTP
                        _http.DefaultRequestHeaders.Authorization = 
                            new AuthenticationHeaderValue("Bearer", authResponse.Token);
                        
                        Console.WriteLine(" Login successful, token stored");
                        return authResponse;
                    }
                }
                Console.WriteLine(" Login failed: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Login error: " + ex.Message);
            }
            return null;
        }

        public async Task<AuthResponse?> Register(RegisterRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/register", request);
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (authResponse != null)
                    {
                        // Stocker le token et les infos utilisateur
                        await _localStorage.SetItemAsync(TOKEN_KEY, authResponse.Token);
                        await _localStorage.SetItemAsync(USER_KEY, authResponse);
                        
                        // Ajouter le token aux headers HTTP
                        _http.DefaultRequestHeaders.Authorization = 
                            new AuthenticationHeaderValue("Bearer", authResponse.Token);
                        
                        Console.WriteLine(" Registration successful, token stored");
                        return authResponse;
                    }
                }
                Console.WriteLine(" Registration failed: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Registration error: " + ex.Message);
            }
            return null;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(TOKEN_KEY);
            await _localStorage.RemoveItemAsync(USER_KEY);
            _http.DefaultRequestHeaders.Authorization = null;
            Console.WriteLine(" Logged out, token removed");
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
                Console.WriteLine(" InitializeAuth error: " + ex.Message);
            }
        }

        public async Task<bool> IsAuthenticated()
        {
            var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
            var isAuth = !string.IsNullOrEmpty(token);
            Console.WriteLine($" IsAuthenticated: {isAuth}, Token: {(token != null ? "exists" : "null")}");
            return isAuth;
        }

        public async Task<string?> GetRole()
        {
            var user = await _localStorage.GetItemAsync<AuthResponse>(USER_KEY);
            Console.WriteLine($" GetRole: {user?.Role ?? "null"}, Email: {user?.Email ?? "null"}");
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
}
