using FTELMarket.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FTELMarket.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(FTELMarketDbContext db) : ControllerBase
{

    public record ProductResponse(
        int Id, string Name, string? Description, decimal Price,
        int Stock, int CategoryId, DateTime CreatedAt, string CategoryName
    );

    [HttpGet(Name ="GetProducts")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> Get()
    {
        var products = await db.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new ProductResponse(
                p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId,
                p.CreatedAt, p.Category.Name))
            .ToListAsync();

        return Ok(products);
    }

    [HttpPost(Name = "PostProducts")]
    public async Task<ActionResult<ProductResponse>> Post(ProductRequest request)
    {
        var category = await db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId);

        if (category is null)
        {
            return BadRequest(new { message = "Cette catégorie n'existe pas." });
        }


        Product product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        var response = new ProductResponse(
            product.Id, product.Name, product.Description, product.Price, 
            product.Stock, product.CategoryId, product.CreatedAt, category.Name
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id}", Name = "PutProducts")]
    public async Task<ActionResult<ProductResponse>> Put(ProductRequest request, int id)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);

        var category = await db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId);


        if (product is null)
        {
            return NotFound();
        }
        if (category is null)
        {
            return BadRequest(new { message = "Cette catégorie n'existe pas." });
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;
        
        await db.SaveChangesAsync();

        var response = new ProductResponse(
            product.Id, product.Name, product.Description, product.Price, 
            product.Stock, product.CategoryId, product.CreatedAt, category.Name
        );

        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteProducts")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return NotFound();

        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return NoContent();
    }
}