using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MyDotNetApp.BlazorClient;
using MyDotNetApp.BlazorClient.Services;
using Blazored.LocalStorage;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5222";


builder.Services.AddHttpClient("apiservice", client =>
        client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler(sp =>
        sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { apiBaseUrl }
            ));


builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("apiservice"));


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<NotificationService>();


builder.Services.AddOidcAuthentication(options =>
{
    var provider = options.ProviderOptions;
    provider.Authority = "http://localhost:8080/realms/car-rental"; 
    provider.ClientId = "blazor-client";                            
    provider.ResponseType = "code";

    provider.DefaultScopes.Add("openid");
    provider.DefaultScopes.Add("profile");
    provider.DefaultScopes.Add("roles");
});

await builder.Build().RunAsync();