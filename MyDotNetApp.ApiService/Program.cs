using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Active les defaults fournis par MyDotNetApp.ServiceDefaults (health, discovery, OTEL, resilience)
builder.AddServiceDefaults();

// -------------------
// 🔧 CONFIGURATION DU CORS (pour Blazor)
// -------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        // Ici, le frontend Blazor sera servi depuis le même domaine ou localhost
        policy.AllowAnyOrigin() // ou mettre l'URL de ton client Blazor si besoin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// -------------------
// 🔧 CONFIGURATION DE LA BASE DE DONNÉES
// -------------------
var connection = builder.Configuration.GetConnectionString("AppDb");
builder.Services.AddDbContext<RentalContext>(opt => opt.UseNpgsql(connection));

// -------------------
// 🚀 CONFIGURATION DES CONTROLLERS ET SWAGGER
// -------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyDotNetApp.ApiService",
        Version = "v1",
        Description = "API de gestion de location de voitures de luxe"
    });
});

// -------------------
// 🏗️ CONSTRUCTION DE L’APPLICATION
// -------------------
var app = builder.Build();

// Mappe les endpoints par défaut (health/alive) définis dans les extensions
app.MapDefaultEndpoints();

// -------------------
// 🚪 MIDDLEWARES
// -------------------
app.UseCors("AllowBlazorClient"); // CORS pour Blazor

// Swagger (uniquement en dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyDotNetApp.ApiService v1"));
}

app.UseRouting();

// -------------------
// ⚙️ MAPPE LES CONTROLLERS
// -------------------
app.MapControllers();

// -------------------
// 🏁 DÉMARRAGE
// -------------------
app.Run();
