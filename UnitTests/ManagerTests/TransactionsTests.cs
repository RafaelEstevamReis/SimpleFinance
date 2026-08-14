namespace UnitTests.ManagerTests;

using Simple.Finance;
using Simple.Finance.Tables;
using System;
using System.Linq;
using System.Threading;
using Xunit;

public class TransactionsTests : ManagerTestBase
{
    /* Validation */

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithoutDescription_Throws(string? description)
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);

        var t = tx(walletId, categoryId, 10m, past);
        t.Description = description!;

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateTransaction(t));
        Assert.Contains(nameof(Transac.Description), ex.Message);
    }

    [Fact]
    public void CreateWalletTransfer_WithoutDescription_Throws()
    {
        // transfers are written through the same path, so they answer to the same rule
        var source = newWallet("Source");
        var destination = newWallet("Destination");

        var ex = Assert.Throws<InvalidOperationException>(()
            => mgr.CreateWalletTransfer(source, 0, destination, 0, "", 100m, past, past, paid: true, paymentDetails: null));

        Assert.Contains(nameof(Transac.Description), ex.Message);
        Assert.Empty(mgr.GetTransactions(Manager.SearchTransactionsDate.DueDate, past.AddDays(-1), past.AddDays(1)));
    }

    [Fact]
    public void Create_WithZeroDueValue_Throws()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);

        var ex = Assert.Throws<InvalidOperationException>(() => newTx(walletId, categoryId, 0m, past));
        Assert.Contains(nameof(Transac.DueValue), ex.Message);
    }

    [Fact]
    public void Create_WithUnknownWallet_Throws()
    {
        var categoryId = newCategory(isExpense: true);

        Assert.Throws<InvalidOperationException>(() => newTx(walletId: 999, categoryId, 10m, past));
    }

    [Fact]
    public void Create_WithUnknownCategory_Throws()
    {
        var walletId = newWallet();

        Assert.Throws<InvalidOperationException>(() => newTx(walletId, categoryId: 999, 10m, past));
    }

    [Fact]
    public void Create_WithoutCategory_Throws()
    {
        var walletId = newWallet();

        Assert.Throws<InvalidOperationException>(() => newTx(walletId, categoryId: 0, 10m, past));
        Assert.Empty(mgr.GetTransactions(Manager.SearchTransactionsDate.DueDate, past.AddDays(-1), past.AddDays(1)));
    }

    [Fact]
    public void CreateBulk_WithoutCategory_Throws()
    {
        // the rule is checked on the bulk entry too, not only on the single one
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);

        Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateBulkTransaction([
            tx(walletId, categoryId: 0, 10m, past),
            tx(walletId, categoryId, 20m, past),
        ]));
        Assert.Empty(mgr.GetTransactions(Manager.SearchTransactionsDate.DueDate, past.AddDays(-1), past.AddDays(1)));
    }

    [Fact]
    public void Create_WithUnknownCounterparty_Throws()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);

        var t = tx(walletId, categoryId, 10m, past);
        t.CounterpartyId = 999;

        Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateTransaction(t));
    }

    [Fact]
    public void Create_WithKnownCounterparty_Persists()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);
        var personId = newPerson();

        var t = tx(walletId, categoryId, 10m, past);
        t.CounterpartyId = personId;
        var id = mgr.CreateUpdateTransaction(t);

        Assert.Equal(personId, mgr.GetTransactionById(id)!.CounterpartyId);
    }

    [Fact]
    public void GetTransactionById_UnknownId_ReturnsNull()
    {
        Assert.Null(mgr.GetTransactionById(999));
    }

    /* Sign normalization */

    [Theory]
    [InlineData(100)]
    [InlineData(-100)]
    public void Create_WithExpenseCategory_ForcesNegativeValues(decimal inputValue)
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);

        var t = tx(walletId, categoryId, inputValue, past);
        t.RC_DueValue = inputValue;
        t.RC_PaidValue = inputValue;
        var id = mgr.CreateUpdateTransaction(t);

        var stored = mgr.GetTransactionById(id)!;
        Assert.Equal(-100m, stored.DueValue);
        Assert.Equal(-100m, stored.PaidValue);
        Assert.Equal(-100m, stored.RC_DueValue);
        Assert.Equal(-100m, stored.RC_PaidValue);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(-100)]
    public void Create_WithIncomeCategory_ForcesPositiveValues(decimal inputValue)
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);

        var id = newTx(walletId, categoryId, inputValue, past);

        var stored = mgr.GetTransactionById(id)!;
        Assert.Equal(100m, stored.DueValue);
        Assert.Equal(100m, stored.PaidValue);
    }

    /* Currency */

    [Fact]
    public void Create_UppercasesPaymentCurrency()
    {
        var walletId = newWallet(baseCurrency: "BRL");
        var categoryId = newCategory(isExpense: false);

        var t = tx(walletId, categoryId, 10m, past);
        t.PaymentCurrency = "brl";
        var id = mgr.CreateUpdateTransaction(t);

        Assert.Equal("BRL", mgr.GetTransactionById(id)!.PaymentCurrency);
    }

    [Fact]
    public void Create_WithCurrencyDifferentFromWallet_Throws()
    {
        var walletId = newWallet(baseCurrency: "BRL");
        var categoryId = newCategory(isExpense: false);

        var t = tx(walletId, categoryId, 10m, past);
        t.PaymentCurrency = "USD";

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateTransaction(t));
        Assert.Contains("Currency", ex.Message);
    }

    [Fact]
    public void Create_WithoutPaymentCurrency_SkipsCurrencyCheck()
    {
        var walletId = newWallet(baseCurrency: "BRL");
        var categoryId = newCategory(isExpense: false);

        var id = newTx(walletId, categoryId, 10m, past);

        Assert.Equal("", mgr.GetTransactionById(id)!.PaymentCurrency);
    }

    /* Types */

    [Fact]
    public void Create_WithWalletTransferType_Throws()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);

        var t = tx(walletId, categoryId, 10m, past);
        t.Type = Transac.TransactionType.WalletTransfer;

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateTransaction(t));
        Assert.Contains("Transfers", ex.Message);
    }

    [Fact]
    public void Create_WithSpecialType_ThrowsNotImplemented()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);

        var t = tx(walletId, categoryId, 10m, past);
        t.Type = Transac.TransactionType.Special;

        Assert.Throws<NotImplementedException>(() => mgr.CreateUpdateTransaction(t));
    }

    [Fact]
    public void Create_WithUnknownType_Throws()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);

        var t = tx(walletId, categoryId, 10m, past);
        t.Type = (Transac.TransactionType)42;

        Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateTransaction(t));
    }

    /* Update */

    [Fact]
    public void Update_PreservesCreatedAndBumpsChanged()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        var id = newTx(walletId, categoryId, 10m, past);
        var original = mgr.GetTransactionById(id)!;

        Thread.Sleep(10);
        var edited = original with { Description = "edited" };
        mgr.CreateUpdateTransaction(edited);

        var stored = mgr.GetTransactionById(id)!;
        Assert.Equal("edited", stored.Description);
        Assert.Equal(original.Created, stored.Created);
        Assert.True(stored.Changed > original.Changed, "Changed should move forward on update");
    }

    [Fact]
    public void Update_DoesNotDuplicateTheRow()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        var id = newTx(walletId, categoryId, 10m, past);

        mgr.CreateUpdateTransaction(mgr.GetTransactionById(id)! with { Description = "edited" });

        var all = mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Wallet, walletId, Manager.SearchTransactionsDate.DueDate, past.AddDays(-1), past.AddDays(1));
        Assert.Single(all);
    }

    [Fact]
    public void CreateBulk_PersistsEveryTransaction()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);

        var ids = mgr.CreateUpdateBulkTransaction([
            tx(walletId, categoryId, 10m, past),
            tx(walletId, categoryId, 20m, past),
            tx(walletId, categoryId, 30m, past),
        ]).ToArray();

        Assert.Equal(3, ids.Length);
        Assert.Equal(3, ids.Distinct().Count());
        Assert.All(ids, id => Assert.NotNull(mgr.GetTransactionById(id)));
        Assert.Equal(60m, mgr.GetWalletBalance(walletId));
    }

    /* Searching */

    [Fact]
    public void GetTransactions_ByDueDate_FiltersByRange()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        var inside = newTx(walletId, categoryId, 10m, new DateTime(2020, 6, 15, 0, 0, 0, DateTimeKind.Utc));
        newTx(walletId, categoryId, 20m, new DateTime(2021, 6, 15, 0, 0, 0, DateTimeKind.Utc));

        var found = mgr.GetTransactions(Manager.SearchTransactionsDate.DueDate,
                                        new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                        new DateTime(2020, 12, 31, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal(inside, found.Single().Id);
    }

    [Fact]
    public void GetTransactions_ByPaymentDate_ExcludesUnpaid()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        var paid = newTx(walletId, categoryId, 10m, past);
        newTx(walletId, categoryId, 20m, past, Transac.PaymentStatus.Unpaid);

        var found = mgr.GetTransactions(Manager.SearchTransactionsDate.PaymentDate, past.AddDays(-1), past.AddDays(1));

        Assert.Equal(paid, found.Single().Id);
    }

    [Fact]
    public void GetTransactions_ByEffectiveDate_UsesPaymentDateForPaidAndDueDateForUnpaid()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        var inRange = new DateTime(2020, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var outOfRange = new DateTime(2022, 6, 15, 0, 0, 0, DateTimeKind.Utc);

        var paidInRange = mgr.CreateUpdateTransaction(new Transac
        {
            Id = 0,
            WalletId = walletId,
            CategoryId = categoryId,
            Description = "paid inside the window",
            DueDate = outOfRange,
            PaymentDate = inRange,
            DueValue = 10m,
            PaidValue = 10m,
            Status = Transac.PaymentStatus.Paid,
        });
        var unpaidInRange = mgr.CreateUpdateTransaction(new Transac
        {
            Id = 0,
            WalletId = walletId,
            CategoryId = categoryId,
            Description = "unpaid inside the window",
            DueDate = inRange,
            PaymentDate = outOfRange,
            DueValue = 20m,
            PaidValue = 0m,
            Status = Transac.PaymentStatus.Unpaid,
        });
        // paid, but settled outside the window
        mgr.CreateUpdateTransaction(new Transac
        {
            Id = 0,
            WalletId = walletId,
            CategoryId = categoryId,
            Description = "paid outside the window",
            DueDate = inRange,
            PaymentDate = outOfRange,
            DueValue = 30m,
            PaidValue = 30m,
            Status = Transac.PaymentStatus.Paid,
        });

        var found = mgr.GetTransactions(Manager.SearchTransactionsDate.EffectiveDate,
                                        new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                        new DateTime(2020, 12, 31, 0, 0, 0, DateTimeKind.Utc))
                       .Select(o => o.Id)
                       .OrderBy(o => o)
                       .ToArray();

        Assert.Equal([paidInRange, unpaidInRange], found);
    }

    [Fact]
    public void GetTransactionsBy_FiltersByWalletCategoryAndCounterparty()
    {
        var walletA = newWallet("A");
        var walletB = newWallet("B");
        var categoryA = newCategory(isExpense: false, "A");
        var categoryB = newCategory(isExpense: false, "B");
        var person = newPerson();

        var target = tx(walletA, categoryA, 10m, past);
        target.CounterpartyId = person;
        var targetId = mgr.CreateUpdateTransaction(target);
        newTx(walletB, categoryB, 20m, past);

        var start = past.AddDays(-1);
        var end = past.AddDays(1);

        Assert.Equal(targetId, mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Wallet, walletA, Manager.SearchTransactionsDate.DueDate, start, end).Single().Id);
        Assert.Equal(targetId, mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Category, categoryA, Manager.SearchTransactionsDate.DueDate, start, end).Single().Id);
        Assert.Equal(targetId, mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Counterparty, person, Manager.SearchTransactionsDate.DueDate, start, end).Single().Id);
    }

    [Fact]
    public void GetTransactions_WithUnknownDateType_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => mgr.GetTransactions((Manager.SearchTransactionsDate)42, past, past.AddDays(1)));
    }

    [Fact]
    public void GetTransactionsBy_WithUnknownKind_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => mgr.GetTransactionsBy((Manager.SearchTransactionsByKind)42, 1, Manager.SearchTransactionsDate.DueDate, past, past.AddDays(1)));
    }

    /* Transfers */

    [Fact]
    public void CreateWalletTransfer_LinksBothSidesWithOppositeSigns()
    {
        var source = newWallet("Source");
        var destination = newWallet("Destination");

        mgr.CreateWalletTransfer(source, 0, destination, 0, "Move", 100m, past, past, paid: true, paymentDetails: "wire");

        var (from, to) = transferPair(source);

        Assert.Equal(-100m, from.DueValue);
        Assert.Equal(-100m, from.PaidValue);
        Assert.Equal(100m, to.DueValue);
        Assert.Equal(source, from.WalletId);
        Assert.Equal(destination, to.WalletId);
        Assert.Equal(Transac.TransactionType.WalletTransfer, from.Type);
        Assert.Equal(Transac.TransactionType.WalletTransfer, to.Type);
        Assert.Equal(to.Id, from.TypeOtherId);
        Assert.Equal(from.Id, to.TypeOtherId);
        Assert.Equal("wire", from.PaymentDetails);
    }

    [Fact]
    public void CreateWalletTransfer_MovesTheBalances()
    {
        var source = newWallet("Source");
        var destination = newWallet("Destination");

        mgr.CreateWalletTransfer(source, 0, destination, 0, "Move", 100m, past, past, paid: true, paymentDetails: null);

        Assert.Equal(-100m, mgr.GetWalletBalance(source));
        Assert.Equal(100m, mgr.GetWalletBalance(destination));
    }

    [Fact]
    public void CreateWalletTransfer_WithNonExpenseSourceCategory_Throws()
    {
        var source = newWallet("Source");
        var destination = newWallet("Destination");
        var income = newCategory(isExpense: false, "In");

        var ex = Assert.Throws<ArgumentException>(() => mgr.CreateWalletTransfer(source, income, destination, income, "Move", 100m, past, past, paid: true, paymentDetails: null));
        Assert.Contains("sourceCategory", ex.Message);
    }

    [Fact]
    public void CreateWalletTransfer_WithExpenseDestinationCategory_Throws()
    {
        var source = newWallet("Source");
        var destination = newWallet("Destination");
        var expense = newCategory(isExpense: true, "Out");

        var ex = Assert.Throws<ArgumentException>(() => mgr.CreateWalletTransfer(source, expense, destination, expense, "Move", 100m, past, past, paid: true, paymentDetails: null));
        Assert.Contains("destinationCategory", ex.Message);
    }

    [Fact]
    public void CreateWalletTransfer_WithOnlyOneCategory_Throws()
    {
        var source = newWallet("Source");
        var destination = newWallet("Destination");
        var expense = newCategory(isExpense: true, "Out");

        Assert.Throws<ArgumentException>(() => mgr.CreateWalletTransfer(source, expense, destination, 0, "Move", 100m, past, past, paid: true, paymentDetails: null));
    }

    [Fact]
    public void CreateWalletTransfer_WithMatchingCategories_UsesThem()
    {
        var source = newWallet("Source");
        var destination = newWallet("Destination");
        var expense = newCategory(isExpense: true, "Out");
        var income = newCategory(isExpense: false, "In");

        mgr.CreateWalletTransfer(source, expense, destination, income, "Move", 100m, past, past, paid: true, paymentDetails: null);

        var (from, to) = transferPair(source);
        Assert.Equal(expense, from.CategoryId);
        Assert.Equal(income, to.CategoryId);
    }

    [Fact]
    public void UpdateWalletTransfer_UpdatesBothSides()
    {
        var source = newWallet("Source");
        var destination = newWallet("Destination");
        mgr.CreateWalletTransfer(source, 0, destination, 0, "Move", 100m, past, past, paid: true, paymentDetails: null);
        var (from, _) = transferPair(source);

        var newDate = past.AddDays(10);
        mgr.UpdateWalletTransfer(from.Id, 250m, 250m, newDate, newDate, "Moved more", "pix", Transac.PaymentStatus.Unpaid);

        var (updatedFrom, updatedTo) = transferPair(source);
        Assert.Equal(-250m, updatedFrom.DueValue);
        Assert.Equal(-250m, updatedFrom.PaidValue);
        Assert.Equal(250m, updatedTo.DueValue);
        Assert.Equal(250m, updatedTo.PaidValue);
        Assert.Equal(newDate, updatedFrom.DueDate);
        Assert.Equal(newDate, updatedTo.PaymentDate);
        Assert.Equal("Moved more", updatedTo.Description);
        Assert.Equal("pix", updatedTo.PaymentDetails);
        Assert.Equal(Transac.PaymentStatus.Unpaid, updatedFrom.Status);
        Assert.Equal(Transac.PaymentStatus.Unpaid, updatedTo.Status);
    }

    [Fact]
    public void UpdateWalletTransfer_OnSimpleTransaction_Throws()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        var id = newTx(walletId, categoryId, 10m, past);

        Assert.Throws<ArgumentException>(() => mgr.UpdateWalletTransfer(id, 10m, 10m, past, past, "x", null, Transac.PaymentStatus.Paid));
    }

    [Fact]
    public void GetTransferPair_ReturnsSourceThenDestination()
    {
        var source = newWallet("Source");
        var destination = newWallet("Destination");
        mgr.CreateWalletTransfer(source, 0, destination, 0, "Move", 100m, past, past, paid: true, paymentDetails: null);
        var incoming = mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Wallet, destination, Manager.SearchTransactionsDate.DueDate, past.AddDays(-1), past.AddDays(1)).Single();

        // asked from the destination side, still returns (negative, positive)
        var (from, to) = mgr.GetTransferPair(incoming);

        Assert.Equal(source, from.WalletId);
        Assert.Equal(destination, to.WalletId);
        Assert.True(from.DueValue < 0);
        Assert.True(to.DueValue > 0);
    }

    [Fact]
    public void GetTransferPair_OnSimpleTransaction_Throws()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: false);
        var id = newTx(walletId, categoryId, 10m, past);

        Assert.Throws<ArgumentException>(() => mgr.GetTransferPair(mgr.GetTransactionById(id)!));
    }

    private (Transac from, Transac to) transferPair(long sourceWallet)
    {
        var one = mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Wallet, sourceWallet, Manager.SearchTransactionsDate.DueDate, new DateTime(2000, 1, 1), new DateTime(2100, 1, 1)).Single();
        return mgr.GetTransferPair(one);
    }
}
