namespace FTELMarket;

public class Category 
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public List<Product> Products { get; set; } = [];
}