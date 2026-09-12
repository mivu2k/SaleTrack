
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
