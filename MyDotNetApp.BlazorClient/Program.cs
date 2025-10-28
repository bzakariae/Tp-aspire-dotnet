using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using MyDotNetApp.BlazorClient;
using MyDotNetApp.BlazorClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ✅ Configure le HttpClient pour ton API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5222/") });

// ✅ Active le stockage local
builder.Services.AddBlazoredLocalStorage();

// ✅ Ajoute ton ApiService (important si tu l’utilises dans tes pages)
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();