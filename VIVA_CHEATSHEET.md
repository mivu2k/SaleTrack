# SaleTrack viva cheat sheet

## 30-second introduction

SaleTrack is a .NET 10 Blazor Server application using SQLite. It records products, purchases and sales. Every physical item has a unique serial number. A purchase adds the serial to stock and creates a negative account entry. A sale marks that serial as sold and creates a positive account entry.

## Important answers

**Why SQLite?** It is stored in one local file and does not need a separate database server, so it is suitable for a small student project.

**How is a serial number tracked?** `StockItems.SerialNumber` has a UNIQUE rule. The Track page joins the product, purchase and sale data for that serial.

**How does the account update?** Purchase and sale services use transactions. The stock change and account entry either both save or both fail.

**Why four projects?** Core holds basic classes, Data owns SQLite code, Web owns the UI, and Tests checks calculation logic.

**Is this complete accounting software?** No. It is a student cash ledger. It does not include tax, credit payments, returns or double-entry accounting.

**What would you add next?** Multi-item invoices, user login, reports, backup, returns and proper double-entry accounts.
