using FTELMarket.Data;
using Microsoft.EntityFrameworkCore;

namespace FTELMarket;

public class ProductSyncService (FlakyService flakyService, FTELMarketDbContext context, ILogger<ProductSyncService> logger)
{
    public async Task SyncAsync()
    {
        logger.LogInformation("Début de la synchronisation des produits.");

        try
        {
            var distantProducts = await flakyService.GetProductsAsync();

            logger.LogInformation("{Count} Produits récupérés depuis l'API tierce.", distantProducts.Count);

            var localProducts = await context.Products
                .Where(p => p.ExternalId != null)
                .ToDictionaryAsync(p => p.ExternalId!.Value);

            var distantIds = new HashSet<Guid>();

            int created=0, updated=0;

            var categories = await context.Categories
                .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var product in distantProducts)
            {
                distantIds.Add(product.Id);

                if (!categories.TryGetValue(product.Category.Trim(), out var category))
                {
                    category = new Category { Name = product.Category.Trim() };
                    context.Categories.Add(category);
                    categories[category.Name] = category;
                }

                if (!localProducts.TryGetValue(product.Id, out var local))
                {
                    context.Products.Add(new Product
                    {
                        ExternalId = product.Id,
                        Name = product.Name,
                        Reference = product.Reference,
                        Price = product.Price,
                        Stock = product.Stock,
                        Category = category,
                        CreatedAt = DateTime.UtcNow,
                    });
                    created++;
                }
                else
                {
                    local.Name = product.Name;
                    local.Category = category;
                    local.Price = product.Price;
                    local.Reference = product.Reference;
                    updated++;
                }
            }

            var toDelete = localProducts.Values
                .Where(p => !distantIds.Contains(p.ExternalId!.Value))
                .ToList();
            context.Products.RemoveRange(toDelete);

            await context.SaveChangesAsync();

            logger.LogInformation("Synchronisation terminée : {created} créé(s), {updated} mis à jour et {deleted} supprimé(s).", created, updated, toDelete.Count);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Echec de la synchronisation des produits avec l'API tierce.");
            throw;
        }
    }
}