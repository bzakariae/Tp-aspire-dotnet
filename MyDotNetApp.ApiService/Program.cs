using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Data;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using LuxuryRental.Api.Services;
using Microsoft.AspNetCore.Authentication; 
using System.Text.Json;  
using MyDotNetApp.ApiService.Services;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddScoped<MyDotNetApp.ApiService.Services.DocumentService>();

// CORS: autorise seulement le front Blazor (dev)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5150")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// DB
var connection = builder.Configuration.GetConnectionString("mydotnetdb");
builder.Services.AddDbContext<RentalContext>(opt => opt.UseNpgsql(connection));

// 🔐 Auth JWT via Keycloak (au lieu de clé symétrique locale)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/car-rental";
        options.RequireHttpsMetadata = false; // dev
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8080/realms/car-rental",
            ValidateAudience = false, // ok en dev
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = "preferred_username",
            RoleClaimType = ClaimTypes.Role
        };

    });
builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();


builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Admin", p => p.RequireRole("ROLE_RENTAL_MANAGER"));
});

builder.Services.AddScoped<JwtService>(); // garde si tu l’utilises ailleurs (sinon, tu peux le retirer)

builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger + Bearer
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FuegoCars API",
        Version = "v1",
        Description = "API de gestion de location de voitures de luxe"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services);

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FuegoCars API v1"));
}

app.UseRouting();

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization(); 

app.MapControllers(); // mets [Authorize] sur tes endpoints protégés

app.Run();
