using Microsoft.EntityFrameworkCore;
using MyDotNetApp.ApiService.Data;
using Npgsql; // pour typer l’exception si tu veux

var builder = WebApplication.CreateBuilder(args);

// 1) Connexion + résilience EF Core
builder.Services.AddDbContext<TicketContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("mydotnetdb")!, 
        npgsql => npgsql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null)));

builder.Services.AddOpenApi();
builder.AddServiceDefaults();

var app = builder.Build();

// 2) Appliquer migrations avec retries
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TicketContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup");

    const int maxAttempts = 10;
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            logger.LogInformation("Applying migrations (attempt {Attempt}/{Max})…", attempt, maxAttempts);
            db.Database.Migrate();
            logger.LogInformation("Migrations applied.");
            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(ex, "DB not ready yet. Retrying in 3s (attempt {Attempt}/{Max})…", attempt, maxAttempts);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "OK");
// (optionnel) endpoint de test DB
app.MapGet("/tickets", async (TicketContext db) => await db.Tickets.AsNoTracking().ToListAsync());

app.Run();