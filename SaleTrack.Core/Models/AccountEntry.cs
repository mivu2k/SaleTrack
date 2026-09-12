namespace SaleTrack.Core.Models;

public class AccountEntry
{
    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
}
