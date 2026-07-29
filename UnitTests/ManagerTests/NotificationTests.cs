namespace UnitTests.ManagerTests;

using Simple.Finance;
using Simple.Finance.Tables;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class NotificationTests : ManagerTestBase
{
    private readonly List<(object? sender, ManagerNotificationEventArgs args)> events = [];

    public NotificationTests()
    {
        mgr.EventNotifier += (sender, args) => events.Add((sender, args));
    }

    [Fact]
    public void CreateWallet_NotifiesNew()
    {
        var walletId = newWallet();

        var (sender, args) = Assert.Single(events);
        Assert.Same(mgr, sender);
        Assert.Equal(ManagerNotificationEventArgs.EventNotificationItem.Wallet, args.Item);
        Assert.Equal(ManagerNotificationEventArgs.EventNotificationAction.New, args.Action);
        Assert.Equal(walletId, args.Id);
    }

    [Fact]
    public void UpdateWallet_NotifiesUpdate()
    {
        var walletId = newWallet("Checking");
        events.Clear();

        mgr.CreateUpdateWallet(new Wallet { Id = walletId, Name = "Renamed" });

        var (_, args) = Assert.Single(events);
        Assert.Equal(ManagerNotificationEventArgs.EventNotificationAction.Update, args.Action);
        Assert.Equal(walletId, args.Id);
    }

    [Fact]
    public void CreateCategory_NotifiesCategory()
    {
        var categoryId = newCategory(isExpense: true);

        var (_, args) = Assert.Single(events);
        Assert.Equal(ManagerNotificationEventArgs.EventNotificationItem.Category, args.Item);
        Assert.Equal(categoryId, args.Id);
    }

    [Fact]
    public void CreatePerson_NotifiesPerson()
    {
        var personId = newPerson();

        var (_, args) = Assert.Single(events);
        Assert.Equal(ManagerNotificationEventArgs.EventNotificationItem.Person, args.Item);
        Assert.Equal(personId, args.Id);
    }

    [Fact]
    public void CreateTransaction_NotifiesTransaction()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        events.Clear();

        var txId = newTx(walletId, categoryId, 10m, past);

        var (_, args) = Assert.Single(events);
        Assert.Equal(ManagerNotificationEventArgs.EventNotificationItem.Transaction, args.Item);
        Assert.Equal(txId, args.Id);
    }

    [Fact]
    public void CreateBulk_NotifiesOncePerTransaction()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        events.Clear();

        var ids = mgr.CreateUpdateBulkTransaction([
            tx(walletId, categoryId, 10m, past),
            tx(walletId, categoryId, 20m, past),
        ]).ToArray();

        Assert.Equal(2, events.Count);
        Assert.All(events, e => Assert.Equal(ManagerNotificationEventArgs.EventNotificationItem.Transaction, e.args.Item));
        Assert.Equal(ids.OrderBy(o => o), events.Select(e => e.args.Id).OrderBy(o => o));
    }

    [Fact]
    public void CreateWalletTransfer_NotifiesBothSides()
    {
        var source = newWallet("Source");
        var destination = newWallet("Destination");
        events.Clear();

        mgr.CreateWalletTransfer(source, 0, destination, 0, "Move", 100m, past, past, paid: true, paymentDetails: null);

        Assert.Equal(2, events.Count);
        Assert.All(events, e => Assert.Equal(ManagerNotificationEventArgs.EventNotificationItem.Transaction, e.args.Item));
        Assert.Equal(2, events.Select(e => e.args.Id).Distinct().Count());
    }
}
