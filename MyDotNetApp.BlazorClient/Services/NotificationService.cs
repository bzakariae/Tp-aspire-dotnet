using System.Net.Http.Json;
using MyDotNetApp.BlazorClient.Models;

namespace MyDotNetApp.BlazorClient.Services
{
    public class NotificationService
    {
        private readonly HttpClient _http;

        public NotificationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Notification>> GetNotifications()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Notification>>("api/notifications") ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<int> GetUnreadCount()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<UnreadCountResponse>("api/notifications/unread-count");
                return response?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task MarkAsRead(int id)
        {
            try
            {
                await _http.PutAsync($"api/notifications/{id}/mark-read", null);
            }
            catch { }
        }

        public async Task MarkAllAsRead()
        {
            try
            {
                await _http.PutAsync("api/notifications/mark-all-read", null);
            }
            catch { }
        }

        public async Task DeleteNotification(int id)
        {
            try
            {
                await _http.DeleteAsync($"api/notifications/{id}");
            }
            catch { }
        }

        private class UnreadCountResponse
        {
            public int Count { get; set; }
        }
    }
}