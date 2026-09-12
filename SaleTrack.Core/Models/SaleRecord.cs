namespace SaleTrack.Core.Models;

public class SaleRecord
{
    public int Id { get; set; }
    public string ProductName { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string Customer { get; set; } = "";
    public decimal SalePrice { get; set; }
    public DateTime SoldOn { get; set; }
}
