using FTELMarket.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace FTELMarket.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(FTELMarketDbContext db) : ControllerBase
{
    public record CategoryResponse(int Id, string Name);

    private readonly FTELMarketDbContext _db = db;

    [HttpGet(Name = "GetCategories")]
    public async Task<ActionResult<IEnumerable<Category>>> Get()
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CategoryResponse(c.Id, c.Name))
            .ToListAsync();

        return Ok(categories);
    }
}