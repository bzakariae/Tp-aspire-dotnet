using MyDotNetApp.ApiService.Controllers.Models;
using Microsoft.EntityFrameworkCore;

namespace MyDotNetApp.ApiService.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RentalContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Vérification de la base de données...");
                
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation($"Application de {pendingMigrations.Count()} migration(s) en attente...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations appliquées avec succès");
                }
                else
                {
                    await context.Database.EnsureCreatedAsync();
                    logger.LogInformation("Base de données vérifiée");
                }

                if (!await context.Users.AnyAsync(u => u.Email == "admin@yassine.com"))
                {
                    var admin = new User
                    {
                        Email = "admin@yassine.com",
                        PasswordHash = "ADMINADMIN",
                        FullName = "Administrator",
                        Role = "Admin",
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Users.Add(admin);
                    await context.SaveChangesAsync();
                }
                else
                {
                    logger.LogInformation("✓ Compte administrateur déjà existant");
                }

                var carCount = await context.Cars.CountAsync();
                logger.LogInformation($"✓ Nombre de voitures dans la base: {carCount}");
                
                logger.LogInformation("========================================");
                logger.LogInformation("Base de données prête !");
                logger.LogInformation("========================================");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données");
                logger.LogError("Conseil: Si les tables existent déjà, supprimez la migration et recréez-la:");
                logger.LogError("  1. Supprimez le dossier Migrations/");
                logger.LogError("  2. dotnet ef migrations add InitialCreate");
                logger.LogError("  3. dotnet run");
                throw;
            }
        }
    }
}
