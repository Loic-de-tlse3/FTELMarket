using FTELMarket.Data;
using Microsoft.EntityFrameworkCore;

namespace FTELMarket;

public class ClientSyncService (FlakyService flakyService, FTELMarketDbContext context, ILogger<ClientSyncService> logger)
{
    public async Task SyncAsync()
    {
        logger.LogInformation("Début de la synchronisation des clients.");

        try{
            var distantClients = await flakyService.GetClientsAsync();

            logger.LogInformation("{Count} Clients récupérés depuis l'API tierce.", distantClients.Count);

            var localClients = await context.Clients.ToDictionaryAsync(c => c.Id);

            var distantIds = new HashSet<Guid>();

            int created=0, updated=0;

            foreach (var client in distantClients)
            {
                distantIds.Add(client.Id);

                if (!localClients.TryGetValue(client.Id, out var local))
                {
                    context.Clients.Add(new Client
                    {
                        Id = client.Id,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Email = client.Email,
                        City = client.City
                    });
                    created++;
                }
                else
                {
                    local.FirstName = client.FirstName;
                    local.LastName = client.LastName;
                    local.Email = client.Email;
                    local.City = client.City;
                    updated++;
                }
            }

            var toDelete = localClients.Values
                .Where(c => !distantIds.Contains(c.Id))
                .ToList();
            context.Clients.RemoveRange(toDelete);

            await context.SaveChangesAsync();

            logger.LogInformation("Synchronisation terminée : {created} Créé(s), {update} mis à jour et {delete} supprimé(s).", created, updated, toDelete.Count);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Échec de la synchronisation des clients avec l'API tierce.");
            throw;
        }
    }
}