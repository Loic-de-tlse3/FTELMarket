namespace FTELMarket;

public class OrderRequest
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = String.Empty;

    public Guid CustomerId { get; set; }

    public Client Client { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = String.Empty;

    public List<OrderLine> Lines { get; set; } = [];
}