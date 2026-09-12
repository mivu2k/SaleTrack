namespace SaleTrack.Core.Models;

// One StockItem represents one physical product with one serial number.
public class StockItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string Supplier { get; set; } = "";
    public decimal CostPrice { get; set; }
    public DateTime PurchasedOn { get; set; }
    public bool IsSold { get; set; }
    public string Customer { get; set; } = "";
    public decimal? SoldPrice { get; set; }
    public DateTime? SoldOn { get; set; }
}
