using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MyDotNetApp.BlazorClient;
using MyDotNetApp.BlazorClient.Services;
using Blazored.LocalStorage;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5222";

// Auth custom
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());

// ➜ Handler qui ajoute le Bearer depuis localStorage
builder.Services.AddTransient<BearerTokenHandler>();

// ➜ HttpClient nommé "apiservice" avec BearerTokenHandler
builder.Services.AddHttpClient("apiservice", c =>
    {
        c.BaseAddress = new Uri(apiBaseUrl);
    })
    .AddHttpMessageHandler<BearerTokenHandler>();

// ➜ HttpClient par défaut = "apiservice"
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("apiservice"));

// Service login Keycloak (utilise HttpClient par défaut, OK)
builder.Services.AddScoped<KeycloakAuthService>();

await builder.Build().RunAsync();