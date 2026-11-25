using FTELMarket;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("sync")]
public class SyncController(ClientSyncService clientSyncService, ProductSyncService productSyncService, OrderSyncService orderSyncService) : ControllerBase
{
    [HttpPost("clients")]
    public async Task<IActionResult> SyncClients()
    {
        await clientSyncService.SyncAsync();
        return Ok("Synchronisation terminée");
    }

    [HttpPost("produits")]
    public async Task<IActionResult> SyncProduits()
    {
        await productSyncService.SyncAsync();
        return Ok("Synchronisation terminée");
    }

    [HttpPost("commandes")]
    public async Task<IActionResult> SyncCommandes()
    {
        await orderSyncService.SyncAsync();
        return Ok("Synchronisation terminée");
    }
}