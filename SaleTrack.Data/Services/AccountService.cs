using Microsoft.Data.Sqlite;
using SaleTrack.Core.Models;

namespace SaleTrack.Data.Services;

public class AccountService
{
    private readonly string _connectionString;

    public AccountService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<AccountEntry>> GetEntriesAsync()
    {
        var entries = new List<AccountEntry>();

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, EntryDate, Type, Description, Amount
            FROM AccountEntries
            ORDER BY Id DESC;
            """;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            entries.Add(new AccountEntry
            {
                Id = reader.GetInt32(0),
                EntryDate = DateTime.Parse(reader.GetString(1)),
                Type = reader.GetString(2),
                Description = reader.GetString(3),
                Amount = Convert.ToDecimal(reader.GetValue(4))
            });
        }

        return entries;
    }

    public async Task<DashboardSummary> GetSummaryAsync()
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT
                (SELECT COUNT(*) FROM Products),
                (SELECT COUNT(*) FROM StockItems WHERE IsSold = 0),
                (SELECT COUNT(*) FROM StockItems WHERE IsSold = 1),
                (SELECT COALESCE(SUM(CostPrice), 0) FROM StockItems),
                (SELECT COALESCE(SUM(SalePrice), 0) FROM Sales),
                (SELECT COALESCE(SUM(Amount), 0) FROM AccountEntries);
            """;

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new DashboardSummary
        {
            Products = reader.GetInt32(0),
            AvailableItems = reader.GetInt32(1),
            SoldItems = reader.GetInt32(2),
            PurchaseTotal = Convert.ToDecimal(reader.GetValue(3)),
            SaleTotal = Convert.ToDecimal(reader.GetValue(4)),
            AccountBalance = Convert.ToDecimal(reader.GetValue(5))
        };
    }
}
