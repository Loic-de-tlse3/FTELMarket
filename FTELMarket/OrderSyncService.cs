using FTELMarket.Data;
using Microsoft.EntityFrameworkCore;

namespace FTELMarket;

public class OrderSyncService (FlakyService flakySercice, FTELMarketDbContext context, ILogger<OrderSyncService> logger) {
    public async Task SyncAsync()
    {
        logger.LogInformation("Début de la synchronisation des commandes.");

        try
        {
            var distantOrders = await flakySercice.GetOrdersAsync();

            logger.LogInformation("{Count} commandes récupérées depuis l'API tierce", distantOrders.Count);

            var localOrders = await context.Orders
                .Include(order => order.Lines)
                .ToDictionaryAsync(order => order.Id);

            var localProductsIds = await context.Products
                .Where(product => product.ExternalId != null)
                .ToDictionaryAsync(product => product.ExternalId!.Value, product => product.Id);
            
            var localClientsIds = (await context.Clients
                .Select(client => client.Id)
                .ToListAsync())
                .ToHashSet();  

            var distantIds = new HashSet<Guid>();

            int created=0, updated=0, skipped=0;

            foreach (var order in distantOrders)
            {
                distantIds.Add(order.Id);

                if (!localClientsIds.Contains(order.CustomerId))
                {
                    skipped++;
                    continue;
                }

                List<OrderLine> orderLines = [];
                var valid = true;

                foreach (var line in order.Lines)
                {
                    if (!localProductsIds.TryGetValue(line.ProductId, out var productId))
                    {
                        valid = false;
                        break;
                    }

                    orderLines.Add(new OrderLine
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        ProductReference = line.ProductReference,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        OrderId = order.Id
                    });
                }

                if (!valid)
                {
                    skipped++;
                    continue;
                }

                if (!localOrders.TryGetValue(order.Id, out var local))
                {

                    context.Orders.Add(new OrderRequest
                    {
                        Id = order.Id,
                        OrderNumber = order.OrderNumber,
                        CustomerId = order.CustomerId,
                        CreatedAt = order.CreatedAt.ToUniversalTime(),
                        Status = order.Status,
                        Lines = orderLines
                    });
                    created++;
                }
                else
                {
                    local.OrderNumber = order.OrderNumber;
                    local.CustomerId = order.CustomerId;
                    local.CreatedAt = order.CreatedAt.ToUniversalTime();
                    local.Status = order.Status;

                    context.OrderLines.RemoveRange(local.Lines);
                    context.OrderLines.AddRange(orderLines);
                    updated++;
                }
            }

            var toDelete = localOrders.Values
                .Where(c => !distantIds.Contains(c.Id))
                .ToList();
            context.Orders.RemoveRange(toDelete);

            await context.SaveChangesAsync();

            logger.LogInformation("Synchronisation terminée : {created} créée(s), {updated} mise(s) à jour, {deleted} supprimée(s) et {skipped} passée(s)", created, updated, toDelete.Count, skipped);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Échec de la synchronisation des commandes avec l'API tierce.");
            throw;
        }
    }
}