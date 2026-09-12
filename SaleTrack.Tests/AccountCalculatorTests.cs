using SaleTrack.Core.Models;
using SaleTrack.Core.Services;
using Xunit;

namespace SaleTrack.Tests;

public class AccountCalculatorTests
{
    [Fact]
    public void Purchase_reduces_balance()
    {
        var entries = new[]
        {
            new AccountEntry { Amount = -50000 }
        };

        Assert.Equal(-50000m, AccountCalculator.GetBalance(entries));
    }

    [Fact]
    public void Sale_increases_balance()
    {
        var entries = new[]
        {
            new AccountEntry { Amount = -50000 },
            new AccountEntry { Amount = 65000 }
        };

        Assert.Equal(15000m, AccountCalculator.GetBalance(entries));
    }

    [Fact]
    public void Empty_account_has_zero_balance()
    {
        Assert.Equal(0m, AccountCalculator.GetBalance(Array.Empty<AccountEntry>()));
    }
}
