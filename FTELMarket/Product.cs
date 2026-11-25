namespace FTELMarket;

public class Product
{
    public Guid Id { get; set; }

    public Guid? ExternalId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Category Category { get; set; } = null!;

    public string? Reference { get; set; }
}