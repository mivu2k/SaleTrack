using Microsoft.Data.Sqlite;
using SaleTrack.Core.Models;

namespace SaleTrack.Data.Services;

public class SaleService
{
    private readonly string _connectionString;

    public SaleService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddAsync(int stockItemId, string customer, decimal salePrice)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        try
        {
            await using var stock = connection.CreateCommand();
            stock.Transaction = transaction;
            stock.CommandText = "UPDATE StockItems SET IsSold = 1 WHERE Id = $id AND IsSold = 0;";
            stock.Parameters.AddWithValue("$id", stockItemId);

            if (await stock.ExecuteNonQueryAsync() == 0)
                throw new Exception("This serial number is already sold or does not exist.");

            await using var sale = connection.CreateCommand();
            sale.Transaction = transaction;
            sale.CommandText = """
                INSERT INTO Sales (StockItemId, Customer, SalePrice, SoldOn)
                VALUES ($stockItemId, $customer, $price, $date);
                """;
            sale.Parameters.AddWithValue("$stockItemId", stockItemId);
            sale.Parameters.AddWithValue("$customer", customer.Trim());
            sale.Parameters.AddWithValue("$price", (double)salePrice);
            sale.Parameters.AddWithValue("$date", DateTime.Now.ToString("s"));
            await sale.ExecuteNonQueryAsync();

            await using var serialCommand = connection.CreateCommand();
            serialCommand.Transaction = transaction;
            serialCommand.CommandText = "SELECT SerialNumber FROM StockItems WHERE Id = $id;";
            serialCommand.Parameters.AddWithValue("$id", stockItemId);
            string serial = (string)(await serialCommand.ExecuteScalarAsync() ?? "Unknown");

            await using var account = connection.CreateCommand();
            account.Transaction = transaction;
            account.CommandText = """
                INSERT INTO AccountEntries (EntryDate, Type, Description, Amount)
                VALUES ($date, 'Sale', $description, $amount);
                """;
            account.Parameters.AddWithValue("$date", DateTime.Now.ToString("s"));
            account.Parameters.AddWithValue("$description", $"Sold serial {serial}");
            account.Parameters.AddWithValue("$amount", (double)salePrice);
            await account.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<SaleRecord>> GetAllAsync()
    {
        var sales = new List<SaleRecord>();

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT s.Id, p.Name, si.SerialNumber, s.Customer, s.SalePrice, s.SoldOn
            FROM Sales s
            JOIN StockItems si ON si.Id = s.StockItemId
            JOIN Products p ON p.Id = si.ProductId
            ORDER BY s.Id DESC;
            """;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            sales.Add(new SaleRecord
            {
                Id = reader.GetInt32(0),
                ProductName = reader.GetString(1),
                SerialNumber = reader.GetString(2),
                Customer = reader.GetString(3),
                SalePrice = Convert.ToDecimal(reader.GetValue(4)),
                SoldOn = DateTime.Parse(reader.GetString(5))
            });
        }

        return sales;
    }
}
