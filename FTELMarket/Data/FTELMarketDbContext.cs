using Microsoft.EntityFrameworkCore;

namespace FTELMarket.Data;

public class FTELMarketDbContext : DbContext
{
    public FTELMarketDbContext(DbContextOptions<FTELMarketDbContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FTELMarketDbContext).Assembly);
    }
}