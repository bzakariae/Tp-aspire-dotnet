using LuxuryRental.Api.Models;
using System.Text;

namespace MyDotNetApp.ApiService.Services
{
    public class DocumentService
    {
        public string GenerateContract(Rental rental)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== CONTRAT DE LOCATION ===");
            sb.AppendLine();
            sb.AppendLine($"Numéro de contrat: {rental.Id}");
            sb.AppendLine($"Date: {DateTime.UtcNow:dd/MM/yyyy}");
            sb.AppendLine();
            sb.AppendLine("LOCATAIRE:");
            sb.AppendLine($"Nom: {rental.RenterName}");
            sb.AppendLine($"Email: {rental.User?.Email ?? "N/A"}");
            sb.AppendLine();
            sb.AppendLine("VÉHICULE:");
            sb.AppendLine($"Marque: {rental.Car?.Make ?? "N/A"}");
            sb.AppendLine($"Modèle: {rental.Car?.Model ?? "N/A"}");
            sb.AppendLine($"Classe: {rental.Car?.Class ?? "N/A"}");
            sb.AppendLine();
            sb.AppendLine("PÉRIODE DE LOCATION:");
            sb.AppendLine($"Date de début: {rental.StartDate:dd/MM/yyyy}");
            sb.AppendLine($"Date de fin: {rental.EndDate:dd/MM/yyyy}");
            sb.AppendLine($"Durée: {(rental.EndDate.ToDateTime(TimeOnly.MinValue) - rental.StartDate.ToDateTime(TimeOnly.MinValue)).Days} jours");
            sb.AppendLine();
            sb.AppendLine("TARIFICATION:");
            sb.AppendLine($"Prix par jour: {rental.Car?.PricePerDay:C} EUR");
            sb.AppendLine($"Prix total: {rental.TotalPrice:C} EUR");
            sb.AppendLine();
            sb.AppendLine("CONDITIONS:");
            sb.AppendLine("- Le locataire s'engage à restituer le véhicule dans l'état où il l'a reçu");
            sb.AppendLine("- Toute détérioration sera facturée au locataire");
            sb.AppendLine("- Le véhicule doit être restitué avec le plein de carburant");
            sb.AppendLine("- Une caution de 1000 EUR sera prélevée et restituée après inspection");
            sb.AppendLine();
            sb.AppendLine("Signature du locataire: _________________");
            sb.AppendLine();
            sb.AppendLine("Signature du loueur: _________________");
            
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        public string GenerateInvoice(Rental rental)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== FACTURE ===");
            sb.AppendLine();
            sb.AppendLine($"Numéro de facture: INV-{rental.Id:D6}");
            sb.AppendLine($"Date d'émission: {DateTime.UtcNow:dd/MM/yyyy}");
            sb.AppendLine();
            sb.AppendLine("LUXURY RENTAL");
            sb.AppendLine("123 Avenue des Champs-Élysées");
            sb.AppendLine("75008 Paris, France");
            sb.AppendLine("SIRET: 123 456 789 00012");
            sb.AppendLine();
            sb.AppendLine("FACTURÉ À:");
            sb.AppendLine($"{rental.RenterName}");
            sb.AppendLine($"{rental.User?.Email ?? "N/A"}");
            sb.AppendLine();
            sb.AppendLine("DÉTAILS DE LA LOCATION:");
            sb.AppendLine("─────────────────────────────────────────────────────────");
            sb.AppendLine($"Véhicule: {rental.Car?.Make} {rental.Car?.Model}");
            sb.AppendLine($"Période: {rental.StartDate:dd/MM/yyyy} - {rental.EndDate:dd/MM/yyyy}");
            
            var days = (rental.EndDate.ToDateTime(TimeOnly.MinValue) - rental.StartDate.ToDateTime(TimeOnly.MinValue)).Days;
            var pricePerDay = rental.Car?.PricePerDay ?? 0;
            
            sb.AppendLine();
            sb.AppendLine("TARIFICATION:");
            sb.AppendLine($"Location ({days} jours × {pricePerDay:C} EUR)     {rental.TotalPrice:C} EUR");
            sb.AppendLine($"TVA (20%)                                {rental.TotalPrice * 0.20m:C} EUR");
            sb.AppendLine("─────────────────────────────────────────────────────────");
            sb.AppendLine($"TOTAL TTC:                               {rental.TotalPrice * 1.20m:C} EUR");
            sb.AppendLine();
            sb.AppendLine("Conditions de paiement: Paiement à la réservation");
            sb.AppendLine("Mode de paiement: Carte bancaire");
            sb.AppendLine();
            sb.AppendLine("Merci de votre confiance !");
            
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
        }
    }
}