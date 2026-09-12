using Microsoft.Data.Sqlite;

namespace SaleTrack.Data.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Brand TEXT NOT NULL,
                SalePrice REAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS StockItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId INTEGER NOT NULL,
                SerialNumber TEXT NOT NULL UNIQUE,
                Supplier TEXT NOT NULL,
                CostPrice REAL NOT NULL,
                PurchasedOn TEXT NOT NULL,
                IsSold INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (ProductId) REFERENCES Products(Id)
            );

            CREATE TABLE IF NOT EXISTS Sales (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StockItemId INTEGER NOT NULL UNIQUE,
                Customer TEXT NOT NULL,
                SalePrice REAL NOT NULL,
                SoldOn TEXT NOT NULL,
                FOREIGN KEY (StockItemId) REFERENCES StockItems(Id)
            );

            CREATE TABLE IF NOT EXISTS AccountEntries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EntryDate TEXT NOT NULL,
                Type TEXT NOT NULL,
                Description TEXT NOT NULL,
                Amount REAL NOT NULL
            );
            """;

        await command.ExecuteNonQueryAsync();
    }
}
