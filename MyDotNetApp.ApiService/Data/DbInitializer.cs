using MyDotNetApp.ApiService.Controllers.Models;
using Microsoft.EntityFrameworkCore;
using MyDotNetApp.ApiService.Services;

namespace MyDotNetApp.ApiService.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RentalContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var keycloakAdmin = scope.ServiceProvider.GetRequiredService<KeycloakAdminService>();
            try
            {
                logger.LogInformation("Application des migrations EF...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations appliquées avec succès");
                const string adminEmail = "admin@car-rental.com";
                // ====== Seed default admin ======
                if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
                {
                    var keycloakId = await keycloakAdmin.CreateAdminAsync(
                        email: adminEmail,
                        firstName: "Admin",
                        lastName: "CarRental",
                        password: "admin-car" 
                    );
                    var admin = new User
                    {
                        Email =adminEmail,
                        FullName = "Admin CarRental",
                        Role = "Admin",
                        KeycloakId = keycloakId,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Users.Add(admin);
                    logger.LogInformation("✓ Compte administrateur créé : admin@car-rental.com");
                }
                else
                {
                    logger.LogInformation("✓ Compte administrateur déjà existant ");
                }

                await context.SaveChangesAsync();

                var carCount = await context.Cars.CountAsync();
                logger.LogInformation($"✓ Nombre de voitures dans la base: {carCount}");
                logger.LogInformation("Base de données prête !");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données");
                throw;
            }
        }
    }
}
