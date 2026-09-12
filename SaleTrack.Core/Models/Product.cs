namespace SaleTrack.Core.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Brand { get; set; } = "";
    public decimal SalePrice { get; set; }
}
