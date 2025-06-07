namespace Simple.Finance;

using Simple.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

public class Manager
{
    private readonly ConnectionFactory db;
    private readonly string dbFile;

    public Manager(string dbFile)
    {
        db = ConnectionFactory.FromFile(dbFile);
        this.dbFile = dbFile;
    }

    /// <summary>
    /// Initializes database and backups it
    /// </summary>
    /// <param name="createBackup">If a backup should be created</param>
    /// <param name="backupName">Backup file name. It will be compressed and '.gz' appended</param>
    public void Initialize(bool createBackup = false, string? backupName = null)
    {
        if (createBackup)
        {
            if (backupName is null) throw new ArgumentNullException(nameof(backupName), "backupName cannot be null");
            compress(dbFile, backupName);
        }

        using var cnn = db.GetConnection();
        cnn.CreateTables()
           .Add<Tables.ChangeLog>()
           .Add<Tables.Category>()
           .Add<Tables.Wallet>()
           .Add<Tables.Person>()
           .Add<Tables.Transac>()
           .Commit();
    }
    static void compress(string dbFile, string destFile)
    {
        var fiOrg = new FileInfo(dbFile);
        if (!fiOrg.Exists) return; // First run

        var fiDest = new FileInfo(destFile);
        using FileStream originalFileStream = File.Open(fiOrg.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using FileStream compressedFileStream = File.Create(fiDest.FullName + ".gz");
        using GZipStream compressionStream = new(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }


    #region Wallets

    public IEnumerable<Tables.Wallet> GetWallets()
    {
        using var cnn = db.GetConnection();
        return cnn.GetAll<Tables.Wallet>();
    }
    public long CreateUpdateWallet(Tables.Wallet wallet)
    {
        using var cnn = db.GetConnection();
        var originalValue = wallet.Id == 0 ? null : cnn.Get<Tables.Wallet>(wallet.Id);

        wallet.Id = cnn.Insert(wallet, OnConflict.Replace);

        saveChangeLog(cnn, originalValue, wallet);
        return wallet.Id;
    }

    public decimal GetWalletBalance(long walletId)
    {
        return 0;
    }

    #endregion

    #region Categories

    public IEnumerable<Tables.Category> GetCategories()
    {
        using var cnn = db.GetConnection();
        return cnn.GetAll<Tables.Category>();
    }
    public long CreateUpdateCategory(Tables.Category category)
    {
        using var cnn = db.GetConnection();
        var originalValue = category.Id == 0 ? null : cnn.Get<Tables.Category>(category.Id);

        // Check for type
        if (category.Id > 0 && originalValue != null)
        {
            if (originalValue.IsExpense != category.IsExpense)
            {
                throw new InvalidOperationException($"'{nameof(Tables.Category.IsExpense)}' cannot be changed");
            }
        }

        category.Id = cnn.Insert(category, OnConflict.Replace);

        saveChangeLog(cnn, originalValue, category);
        return category.Id;
    }

    #endregion

    #region Persons

    public IEnumerable<Tables.Person> GetAllPersons()
    {
        using var cnn = db.GetConnection();
        return cnn.GetAll<Tables.Person>();
    }
    public long CreateUpdatePerson(Tables.Person person)
    {
        using var cnn = db.GetConnection();
        var originalValue = person.Id == 0 ? null : cnn.Get<Tables.Person>(person.Id);

        person.Id = (int)cnn.Insert(person, OnConflict.Replace);

        saveChangeLog(cnn, originalValue, person);
        return person.Id;
    }

    #endregion

    #region Transactions
    public IEnumerable<Tables.Transac> GetTransactions(SearchTransactionsDate dateType, DateTime start, DateTime end)
    {
        string add = "";
        string dateColumn = dateType switch
        {
            SearchTransactionsDate.DueDate => "DueDate",
            SearchTransactionsDate.PaymentDate => "PaymentDate",
            SearchTransactionsDate.Created => "Created",
            SearchTransactionsDate.Changed => "Changed",
            _ => throw new InvalidOperationException("Invalid date type"),
        };

        if (dateType == SearchTransactionsDate.PaymentDate)
        {
            add = "AND PaymentStatus = @statusPaid";
        }

        using var cnn = db.GetConnection();
        return cnn.Query<Tables.Transac>($"SELECT * FROM {nameof(Tables.Transac)} WHERE ({dateColumn} BETWEEN @start AND @end ) {add} ", new
        {
            start,
            end,
            statusPaid = Tables.Transac.PaymentStatus.Paid,
        });
    }
    public IEnumerable<Tables.Transac> GetTransactionsBy(SearchTransactionsByKind kind, long id, SearchTransactionsDate dateType, DateTime start, DateTime end)
    {
        string add = "";
        string kindColumn = kind switch
        {
            SearchTransactionsByKind.Wallet => "WalletId",
            SearchTransactionsByKind.Category => "CategoryId",
            SearchTransactionsByKind.Counterparty => "CounterpartyId",
            _ => throw new InvalidOperationException("Invalid kind type"),
        };
        string dateColumn = dateType switch
        {
            SearchTransactionsDate.DueDate => "DueDate",
            SearchTransactionsDate.PaymentDate => "PaymentDate",
            SearchTransactionsDate.Created => "Created",
            SearchTransactionsDate.Changed => "Changed",
            _ => throw new InvalidOperationException("Invalid date type"),
        };

        if (dateType == SearchTransactionsDate.PaymentDate)
        {
            add = "AND PaymentStatus = @statusPaid";
        }

        using var cnn = db.GetConnection();
        return cnn.Query<Tables.Transac>($"SELECT * FROM {nameof(Tables.Transac)} WHERE {kindColumn} = @id AND ({dateColumn} BETWEEN @start AND @end ) {add} ", new
        {
            id,
            start,
            end,
            statusPaid = Tables.Transac.PaymentStatus.Paid,
        });
    }

    public long CreateUpdateTransaction(Tables.Transac tx)
    {
        using var cnn = db.GetConnection();
        var originalValue = tx.Id == 0 ? null : cnn.Get<Tables.Transac>(tx.Id);

        // Check signs
        if (tx.DueValue == 0) throw new InvalidOperationException($"'{nameof(Tables.Transac.DueValue)}' must not zero");
        var dueSign = Math.Sign(tx.DueValue);
        var pSign = Math.Sign(tx.PaidValue);
        if (pSign != 0 && dueSign != pSign) throw new InvalidOperationException($" Sign of '{nameof(Tables.Transac.DueValue)}' must be equal '{nameof(Tables.Transac.PaidValue)}'");

        // check wallet
        var wallet = cnn.Get<Tables.Wallet>(tx.WalletId);
        if (wallet == null) throw new InvalidOperationException($"Invalid Wallet Id: {tx.WalletId}");

        // Check Category
        var category = cnn.Get<Tables.Category>(tx.CategoryId);
        if (category == null) throw new InvalidOperationException($"Invalid Category Id: {tx.CategoryId}");

        if (dueSign > 0 && category.IsExpense)
        {
            throw new InvalidOperationException($"An 'IsExpense' category can only be used with negative values");
        }
        else if (dueSign < 0 && !category.IsExpense)
        {
            throw new InvalidOperationException($"An 'IsExpense' category can not be used with negative values");
        }

        if (tx.CounterpartyId != 0)
        {
            var cparty = cnn.Get<Tables.Person>(tx.CounterpartyId);
            if (cparty == null) throw new InvalidOperationException($"Invalid Counterparty Id: {tx.CounterpartyId}");
        }

        switch (tx.Type)
        {
            case Tables.Transac.TransactionType.Simple:
                break;
            case Tables.Transac.TransactionType.WalletTransfer:
                throw new InvalidOperationException($"This function cannot be used with Transfers");
            case Tables.Transac.TransactionType.Special:
                throw new NotImplementedException();
            default:
                throw new InvalidOperationException($"Invalid Type");
        }

        tx.Id = (int)cnn.Insert(tx, OnConflict.Replace);

        saveChangeLog(cnn, originalValue, tx);
        return tx.Id;
    }
    public void CreateUpdateWalletTransfer(Tables.Transac send, Tables.Transac receive)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region ChangeLog
    public IEnumerable<Tables.ChangeLog> GetLogs(DateTime start, DateTime end)
    {
        using var cnn = db.GetConnection();
        return cnn.Query<Tables.ChangeLog>("SELECT * FROM ChangeLog WHERE Event BETWEEN @start AND @end ", new
        {
            start,
            end
        });
    }
    public IEnumerable<Tables.ChangeLog> GetLogs<T>(DateTime start, DateTime end)
    {
        var type = typeof(T);
        using var cnn = db.GetConnection();
        return cnn.Query<Tables.ChangeLog>("SELECT * FROM ChangeLog WHERE Table=@table AND (Event BETWEEN @start AND @end) ", new
        {
            table = type.FullName,
            start,
            end
        });
    }
    private static bool saveChangeLog<T>(ISqliteConnection cnn, T? older, T newer)
    {
        var type = typeof(T);
        var diff = Helpers.ModelDiff(older, newer);

        var records = diff.Keys.Select(o => new Tables.ChangeLog
        {
            Id = 0,
            Event = DateTime.UtcNow,
            TableName = type.FullName,
            FieldName = o,
            OldValue = diff[o].Item1,
            NewValue = diff[o].Item2,
        })
        .ToArray();

        cnn.BulkInsert(records);

        return records.Length > 0;
    }

    #endregion

    #region Search Enums
    public enum SearchTransactionsByKind
    {
        Wallet,
        Category,
        Counterparty,
    }
    public enum SearchTransactionsDate
    {
        DueDate,
        PaymentDate,
        Created,
        Changed,
    }

    #endregion

}
