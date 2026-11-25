namespace FTELMarket;

public class FlakyProductRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Reference { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string Category { get; set; } = string.Empty;
}