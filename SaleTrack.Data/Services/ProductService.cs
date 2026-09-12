using Microsoft.Data.Sqlite;
using SaleTrack.Core.Models;

namespace SaleTrack.Data.Services;

public class ProductService
{
    private readonly string _connectionString;

    public ProductService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddAsync(string name, string brand, decimal salePrice)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Products (Name, Brand, SalePrice)
            VALUES ($name, $brand, $salePrice);
            """;
        command.Parameters.AddWithValue("$name", name.Trim());
        command.Parameters.AddWithValue("$brand", brand.Trim());
        command.Parameters.AddWithValue("$salePrice", (double)salePrice);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var products = new List<Product>();

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Brand, SalePrice FROM Products ORDER BY Name;";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Brand = reader.GetString(2),
                SalePrice = Convert.ToDecimal(reader.GetValue(3))
            });
        }

        return products;
    }
}
