using System.ComponentModel.DataAnnotations;

namespace FTELMarket;

public class ProductRequest
{
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0.01", "1000000", 
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}