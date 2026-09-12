using SaleTrack.Data.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

string connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=saletrack.db;Foreign Keys=True";

builder.Services.AddSingleton(new DatabaseService(connectionString));
builder.Services.AddSingleton(new ProductService(connectionString));
builder.Services.AddSingleton(new PurchaseService(connectionString));
builder.Services.AddSingleton(new SaleService(connectionString));
builder.Services.AddSingleton(new AccountService(connectionString));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.Services.GetRequiredService<DatabaseService>().InitializeAsync();
app.Run();
