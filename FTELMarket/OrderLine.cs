namespace FTELMarket;

public class OrderLine
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public OrderRequest Order { get; set; } = null!;

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string ProductReference { get; set; } = String.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}