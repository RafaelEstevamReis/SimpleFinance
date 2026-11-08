namespace UnitTests.ManagerTests;

using Simple.Finance.Tables;
using System;
using System.Linq;
using Xunit;

public class ChangeLogTests : ManagerTestBase
{
    [Fact]
    public void Create_LogsEveryFieldWithNullMarkerAsOldValue()
    {
        var walletId = mgr.CreateUpdateWallet(new Wallet { Id = 0, Name = "Checking", BaseCurrency = "BRL" });

        var log = mgr.GetLogs<Wallet>(walletId).ToArray();

        Assert.All(log, o => Assert.Equal("Wallet", o.TableName));
        Assert.All(log, o => Assert.Equal(walletId, o.TableId));
        Assert.All(log, o => Assert.Equal("[NL]", o.OldValue));
        Assert.Equal("Checking", log.Single(o => o.FieldName == nameof(Wallet.Name)).NewValue);
        Assert.Equal("BRL", log.Single(o => o.FieldName == nameof(Wallet.BaseCurrency)).NewValue);
    }

    [Fact]
    public void Update_LogsOnlyTheChangedFields()
    {
        var walletId = mgr.CreateUpdateWallet(new Wallet { Id = 0, Name = "Checking", Description = "Main" });

        mgr.CreateUpdateWallet(new Wallet { Id = walletId, Name = "Renamed", Description = "Main" });

        var lastEvent = mgr.GetLogs<Wallet>(walletId)
                           .GroupBy(o => o.LogId)
                           .OrderBy(o => o.Key)
                           .Last()
                           .ToArray();

        var changed = Assert.Single(lastEvent);
        Assert.Equal(nameof(Wallet.Name), changed.FieldName);
        Assert.Equal("Checking", changed.OldValue);
        Assert.Equal("Renamed", changed.NewValue);
    }

    [Fact]
    public void Update_WithoutRealChanges_LogsNoFields()
    {
        var walletId = newWallet("Checking");
        var before = mgr.GetLogs<Wallet>(walletId).Count();

        mgr.CreateUpdateWallet(mgr.GetWallets().Single());

        Assert.Equal(before, mgr.GetLogs<Wallet>(walletId).Count());
    }

    [Fact]
    public void Update_FromAFreshModel_DoesNotLogUntouchedDecimals()
    {
        // A client rebuilds the row instead of resaving the one it read, so the decimals
        // arrive with a different scale than the stored ones. Same money, no change to log
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);
        var txId = newTx(walletId, categoryId, 100m, past);

        var rebuilt = tx(walletId, categoryId, 100m, past);
        rebuilt.Id = txId;
        rebuilt.Description = "renamed";
        mgr.CreateUpdateTransaction(rebuilt);

        var lastEvent = mgr.GetLogs<Transac>(txId).GroupBy(o => o.LogId).OrderBy(g => g.Key).Last();
        var changedFields = lastEvent.Select(o => o.FieldName).ToArray();

        Assert.Contains(nameof(Transac.Description), changedFields);
        Assert.DoesNotContain(nameof(Transac.DueValue), changedFields);
        Assert.DoesNotContain(nameof(Transac.PaidValue), changedFields);
        Assert.DoesNotContain(nameof(Transac.RC_DueValue), changedFields);
        Assert.DoesNotContain(nameof(Transac.RC_PaidValue), changedFields);
    }

    [Fact]
    public void GetLogsByType_UsesTheTableNameOfTheType()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        var txId = newTx(walletId, categoryId, 10m, past);

        var log = mgr.GetLogs<Transac>(txId).ToArray();

        Assert.NotEmpty(log);
        Assert.All(log, o => Assert.Equal("Transac", o.TableName));
        Assert.All(log, o => Assert.Equal(txId, o.TableId));
    }

    [Fact]
    public void GetLogsByType_WithInvalidId_ReturnsEmpty()
    {
        newWallet();

        Assert.Empty(mgr.GetLogs<Wallet>(0));
        Assert.Empty(mgr.GetLogs<Wallet>(-1));
    }

    [Fact]
    public void GetLogsByDate_ReturnsOnlyEventsInsideTheWindow()
    {
        var walletId = newWallet();

        Assert.Contains(mgr.GetLogs(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow.AddMinutes(5)), o => o.TableId == walletId && o.TableName == "Wallet");
        Assert.Empty(mgr.GetLogs(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)));
    }

    [Fact]
    public void ExternalId_IsStampedOnEveryLogOfTheCurrentContext()
    {
        mgr.EventLogCurrentExternalId = 42;
        var walletId = newWallet();

        Assert.All(mgr.GetLogs<Wallet>(walletId), o => Assert.Equal(42, o.ExternalId));
    }

    [Fact]
    public void GetLogsByExternalId_FiltersByTheParentEventAuthor()
    {
        mgr.EventLogCurrentExternalId = 10;
        var walletOfUser10 = newWallet("Ten");
        mgr.EventLogCurrentExternalId = 20;
        var walletOfUser20 = newWallet("Twenty");

        var start = DateTime.UtcNow.AddMinutes(-5);
        var end = DateTime.UtcNow.AddMinutes(5);

        var logsOf10 = mgr.GetLogs(start, end, 10).ToArray();

        Assert.NotEmpty(logsOf10);
        Assert.All(logsOf10, o => Assert.Equal(10, o.ExternalId));
        Assert.All(logsOf10, o => Assert.Equal(walletOfUser10, o.TableId));
        Assert.DoesNotContain(logsOf10, o => o.TableId == walletOfUser20);
    }
}
