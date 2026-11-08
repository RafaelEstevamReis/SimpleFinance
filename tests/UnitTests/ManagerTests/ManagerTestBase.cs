namespace UnitTests.ManagerTests;

using Simple.Finance;
using Simple.Finance.Tables;
using System;
using System.IO;

/// <summary>
/// Gives every test its own initialized SQLite file.
/// xUnit builds a new instance per test, so no state leaks between tests.
/// </summary>
public abstract class ManagerTestBase : IDisposable
{
    /// <summary>A date safely in the past for any timezone, used for 'already settled' rows</summary>
    protected static readonly DateTime past = new(2020, 1, 15, 12, 0, 0, DateTimeKind.Utc);

    protected readonly string dbFile;
    protected readonly Manager mgr;

    protected ManagerTestBase()
    {
        dbFile = Path.Combine(Path.GetTempPath(), $"simplefinance_{Guid.NewGuid():N}.db");
        mgr = new Manager(dbFile);
        mgr.Initialize();
    }

    protected long newWallet(string name = "Wallet", string baseCurrency = "")
        => mgr.CreateUpdateWallet(new Wallet
        {
            Id = 0,
            Name = name,
            Description = "",
            BaseCurrency = baseCurrency,
        });

    protected long newCategory(bool isExpense, string name = "Category")
        => mgr.CreateUpdateCategory(new Category
        {
            Id = 0,
            Name = name,
            Description = "",
            IsExpense = isExpense,
        });

    protected long newPerson(string name = "Person")
        => mgr.CreateUpdatePerson(new Person
        {
            Id = 0,
            Name = name,
        });

    protected static Transac tx(long walletId, long categoryId, decimal value, DateTime date, Transac.PaymentStatus status = Transac.PaymentStatus.Paid)
        => new()
        {
            Id = 0,
            WalletId = walletId,
            CategoryId = categoryId,
            Description = "tx",
            DueDate = date,
            PaymentDate = date,
            DueValue = value,
            PaidValue = value,
            Status = status,
        };

    protected long newTx(long walletId, long categoryId, decimal value, DateTime date, Transac.PaymentStatus status = Transac.PaymentStatus.Paid)
        => mgr.CreateUpdateTransaction(tx(walletId, categoryId, value, date, status));

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        try
        {
            File.Delete(dbFile);
        }
        catch (IOException) { /* temp file, best effort */ }
    }
}
