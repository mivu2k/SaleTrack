# SaleTrack — Student Edition

SaleTrack is a small .NET 10 Blazor Server application for products, serialized purchases, sales and a basic cash account. It uses a local SQLite database, so no separate database server is required.

## Easy flow

1. Add a product.
2. Record a purchase with its unique serial number.
3. The item appears as available and its cost is entered as an account debit.
4. Sell the available serial number.
5. The item becomes sold and the sale is entered as an account credit.
6. Search the serial number to see its full purchase and sale history.

## Four projects

- `SaleTrack.Core`: simple models and account calculations.
- `SaleTrack.Data`: SQLite tables and SQL commands.
- `SaleTrack.Web`: Blazor pages.
- `SaleTrack.Tests`: three easy unit tests.

## Run

```powershell
dotnet restore SaleTrack.sln
dotnet build SaleTrack.sln
dotnet test SaleTrack.Tests
dotnet run --project SaleTrack.Web
```

Open `https://localhost:7101`. The database file is created automatically.

Read `docs/SaleTrack_Plain_Code_Guide.html` for complete code, GitHub, CI/CD, Docker and viva guidance.
