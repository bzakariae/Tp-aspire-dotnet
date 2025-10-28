using System.Net.Http.Json;
using MyDotNetApp.BlazorClient.Models;

namespace MyDotNetApp.BlazorClient.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Car>> GetAvailableCars()
        {
            return await _http.GetFromJsonAsync<List<Car>>("renter/cars") ?? new();
        }

        public async Task<bool> RentCar(RentalRequest request)
        {
            var response = await _http.PostAsJsonAsync("renter/rent", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Rental>> GetMyRentals(string renterName)
        {
            return await _http.GetFromJsonAsync<List<Rental>>($"renter/myrentals?renterName={renterName}") ?? new();
        }
    }
}