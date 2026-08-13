namespace UnitTests.ManagerTests;

using Simple.Finance.Tables;
using System;
using System.Linq;
using Xunit;

public class WalletsTests : ManagerTestBase
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateWallet_WithoutName_Throws(string? name)
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateWallet(new Wallet
        {
            Id = 0,
            Name = name!,
            Description = "no name, still a wallet",
        }));

        Assert.Contains(nameof(Wallet.Name), ex.Message);
        Assert.Empty(mgr.GetWallets());
    }

    [Fact]
    public void CreateWallet_AssignsIdAndPersists()
    {
        var id = mgr.CreateUpdateWallet(new Wallet
        {
            Id = 0,
            Name = "Checking",
            Description = "Main account",
            BaseCurrency = "BRL",
        });

        Assert.True(id > 0);

        var stored = mgr.GetWallets().Single();
        Assert.Equal(id, stored.Id);
        Assert.Equal("Checking", stored.Name);
        Assert.Equal("Main account", stored.Description);
        Assert.Equal("BRL", stored.BaseCurrency);
        Assert.False(stored.IsDeleted);
    }

    [Fact]
    public void UpdateWallet_KeepsIdAndDoesNotDuplicate()
    {
        var id = newWallet("Checking");

        var updatedId = mgr.CreateUpdateWallet(new Wallet
        {
            Id = id,
            Name = "Renamed",
            Description = "",
            BaseCurrency = "USD",
        });

        Assert.Equal(id, updatedId);
        var stored = mgr.GetWallets().Single();
        Assert.Equal("Renamed", stored.Name);
        Assert.Equal("USD", stored.BaseCurrency);
    }

    [Fact]
    public void GetWallets_ReturnsAllIncludingDeletedOnes()
    {
        var alive = newWallet("Alive");
        var deleted = mgr.CreateUpdateWallet(new Wallet { Id = 0, Name = "Gone", IsDeleted = true });

        var ids = mgr.GetWallets().Select(o => o.Id).ToArray();

        Assert.Equal([alive, deleted], ids.OrderBy(o => o).ToArray());
    }

    [Fact]
    public void GetWalletBalance_WithoutTransactions_ReturnsZero()
    {
        var walletId = newWallet();

        Assert.Equal(0m, mgr.GetWalletBalance(walletId));
    }

    [Fact]
    public void GetWalletBalance_SumsPaidTransactionsOnly()
    {
        var walletId = newWallet();
        var income = newCategory(isExpense: false, "Salary");
        var expense = newCategory(isExpense: true, "Rent");

        newTx(walletId, income, 100m, past);
        newTx(walletId, expense, 25.5m, past);
        newTx(walletId, expense, 40m, past, Transac.PaymentStatus.Unpaid);

        Assert.Equal(74.5m, mgr.GetWalletBalance(walletId));
    }

    [Fact]
    public void GetWalletBalance_IgnoresPaymentsDatedInTheFuture()
    {
        var walletId = newWallet();
        var income = newCategory(isExpense: false);

        newTx(walletId, income, 100m, past);
        newTx(walletId, income, 500m, DateTime.UtcNow.AddDays(30));

        Assert.Equal(100m, mgr.GetWalletBalance(walletId));
    }

    [Fact]
    public void GetWalletBalance_IsScopedToASingleWallet()
    {
        var walletA = newWallet("A");
        var walletB = newWallet("B");
        var income = newCategory(isExpense: false);

        newTx(walletA, income, 100m, past);
        newTx(walletB, income, 250m, past);

        Assert.Equal(100m, mgr.GetWalletBalance(walletA));
        Assert.Equal(250m, mgr.GetWalletBalance(walletB));
    }

    [Fact]
    public void GetWalletsBalance_GroupsPaidValuesByWallet()
    {
        var walletA = newWallet("A");
        var walletB = newWallet("B");
        var income = newCategory(isExpense: false);
        var expense = newCategory(isExpense: true);

        newTx(walletA, income, 100m, past);
        newTx(walletA, expense, 30m, past);
        newTx(walletB, income, 250m, past);
        newTx(walletB, income, 10m, past, Transac.PaymentStatus.Unpaid);

        var balances = mgr.GetWalletsBalance().ToDictionary(o => o.WalletId, o => o.Balance);

        Assert.Equal(70m, balances[walletA]);
        Assert.Equal(250m, balances[walletB]);
    }

    [Fact]
    public void GetWalletsBalanceAtDate_AddsDuesFallingBeforeThatDate()
    {
        var walletId = newWallet();
        var income = newCategory(isExpense: false);

        newTx(walletId, income, 100m, past);
        newTx(walletId, income, 50m, DateTime.UtcNow.AddDays(3), Transac.PaymentStatus.Unpaid);
        newTx(walletId, income, 900m, DateTime.UtcNow.AddDays(30), Transac.PaymentStatus.Unpaid);

        var balance = mgr.GetWalletsBalance(DateTime.UtcNow.AddDays(7)).Single(o => o.WalletId == walletId);

        Assert.Equal(150m, balance.Balance);
    }
}
