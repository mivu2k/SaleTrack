namespace SaleTrack.Core.Models;

public class DashboardSummary
{
    public int Products { get; set; }
    public int AvailableItems { get; set; }
    public int SoldItems { get; set; }
    public decimal PurchaseTotal { get; set; }
    public decimal SaleTotal { get; set; }
    public decimal AccountBalance { get; set; }
}
