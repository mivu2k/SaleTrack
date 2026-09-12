using SaleTrack.Core.Models;

namespace SaleTrack.Core.Services;

public static class AccountCalculator
{
    public static decimal GetBalance(IEnumerable<AccountEntry> entries)
    {
        return entries.Sum(entry => entry.Amount);
    }

    public static decimal GetDebit(AccountEntry entry)
    {
        return entry.Amount < 0 ? Math.Abs(entry.Amount) : 0;
    }

    public static decimal GetCredit(AccountEntry entry)
    {
        return entry.Amount > 0 ? entry.Amount : 0;
    }
}
