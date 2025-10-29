using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyDotNetApp.BlazorClient;
using MyDotNetApp.BlazorClient.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5222") });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthService>();

var host = builder.Build();

var authService = host.Services.GetRequiredService<AuthService>();
await authService.InitializeAuth();

await host.RunAsync();