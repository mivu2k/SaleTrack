using Microsoft.Data.Sqlite;
using SaleTrack.Core.Models;

namespace SaleTrack.Data.Services;

public class PurchaseService
{
    private readonly string _connectionString;

    public PurchaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddAsync(int productId, string serialNumber, string supplier, decimal costPrice)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        try
        {
            await using var purchase = connection.CreateCommand();
            purchase.Transaction = transaction;
            purchase.CommandText = """
                INSERT INTO StockItems
                    (ProductId, SerialNumber, Supplier, CostPrice, PurchasedOn, IsSold)
                VALUES
                    ($productId, $serial, $supplier, $cost, $date, 0);
                """;
            purchase.Parameters.AddWithValue("$productId", productId);
            purchase.Parameters.AddWithValue("$serial", serialNumber.Trim());
            purchase.Parameters.AddWithValue("$supplier", supplier.Trim());
            purchase.Parameters.AddWithValue("$cost", (double)costPrice);
            purchase.Parameters.AddWithValue("$date", DateTime.Now.ToString("s"));
            await purchase.ExecuteNonQueryAsync();

            await using var account = connection.CreateCommand();
            account.Transaction = transaction;
            account.CommandText = """
                INSERT INTO AccountEntries (EntryDate, Type, Description, Amount)
                VALUES ($date, 'Purchase', $description, $amount);
                """;
            account.Parameters.AddWithValue("$date", DateTime.Now.ToString("s"));
            account.Parameters.AddWithValue("$description", $"Purchased serial {serialNumber.Trim()}");
            account.Parameters.AddWithValue("$amount", (double)-costPrice);
            await account.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<StockItem>> GetAllAsync(bool availableOnly = false)
    {
        var items = new List<StockItem>();

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT si.Id, si.ProductId, p.Name, si.SerialNumber, si.Supplier,
                   si.CostPrice, si.PurchasedOn, si.IsSold,
                   COALESCE(s.Customer, ''), s.SalePrice, s.SoldOn
            FROM StockItems si
            JOIN Products p ON p.Id = si.ProductId
            LEFT JOIN Sales s ON s.StockItemId = si.Id
            WHERE ($availableOnly = 0 OR si.IsSold = 0)
            ORDER BY si.Id DESC;
            """;
        command.Parameters.AddWithValue("$availableOnly", availableOnly ? 1 : 0);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            items.Add(ReadItem(reader));

        return items;
    }

    public async Task<StockItem?> FindBySerialAsync(string serialNumber)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT si.Id, si.ProductId, p.Name, si.SerialNumber, si.Supplier,
                   si.CostPrice, si.PurchasedOn, si.IsSold,
                   COALESCE(s.Customer, ''), s.SalePrice, s.SoldOn
            FROM StockItems si
            JOIN Products p ON p.Id = si.ProductId
            LEFT JOIN Sales s ON s.StockItemId = si.Id
            WHERE si.SerialNumber = $serial;
            """;
        command.Parameters.AddWithValue("$serial", serialNumber.Trim());

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadItem(reader) : null;
    }

    private static StockItem ReadItem(SqliteDataReader reader)
    {
        return new StockItem
        {
            Id = reader.GetInt32(0),
            ProductId = reader.GetInt32(1),
            ProductName = reader.GetString(2),
            SerialNumber = reader.GetString(3),
            Supplier = reader.GetString(4),
            CostPrice = Convert.ToDecimal(reader.GetValue(5)),
            PurchasedOn = DateTime.Parse(reader.GetString(6)),
            IsSold = reader.GetInt32(7) == 1,
            Customer = reader.GetString(8),
            SoldPrice = reader.IsDBNull(9) ? null : Convert.ToDecimal(reader.GetValue(9)),
            SoldOn = reader.IsDBNull(10) ? null : DateTime.Parse(reader.GetString(10))
        };
    }
}
