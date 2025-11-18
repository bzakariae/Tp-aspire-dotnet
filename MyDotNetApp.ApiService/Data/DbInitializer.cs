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
                logger.LogInformation("Application des migrations EF...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations appliquées avec succès");

                // ====== Seed admin ======
                if (!await context.Users.AnyAsync(u => u.Email == "admin@car-rental.com"))
                {
                    var admin = new User
                    {
                        Email = "admin@car-rental.com",
                        FullName = "Admin CarRental",
                        Role = "Admin",
                        KeycloakId = "",
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Users.Add(admin);
                    logger.LogInformation("✓ Compte administrateur créé : admin@car-rental.com");
                }
                else
                {
                    logger.LogInformation("✓ Compte administrateur déjà existant (admin@car-rental.com)");
                }

                // ====== Seed client ======
                if (!await context.Users.AnyAsync(u => u.Email == "client1@gmail.com"))
                {
                    var client = new User
                    {
                        Email = "client1@gmail.com",
                        FullName = "Client Démo",
                        Role = "Client",
                        KeycloakId = "",
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Users.Add(client);
                    logger.LogInformation("✓ Compte client créé : client1@gmail.com");
                }
                else
                {
                    logger.LogInformation("✓ Compte client déjà existant (client1@gmail.com)");
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
