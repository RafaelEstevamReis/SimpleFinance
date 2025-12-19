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

    /// <summary>
    /// Triggers an event when a item is updated in this instance
    /// </summary>
    public event EventHandler<ManagerNotificationEventArgs>? EventNotifier;

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
           .Add<Tables.ChangeLogItem>()
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
        var zipName = fiDest.FullName + ".gz";

        using FileStream originalFileStream = File.Open(fiOrg.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using FileStream compressedFileStream = File.Create(zipName);
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
        using var cnn = db.GetConnection();
        return cnn.ExecuteScalar<decimal>("SELECT COALESCE(SUM(PaidValue),0) FROM Transac WHERE WalletId = @id AND Status = @status AND PaymentDate <= CURRENT_TIMESTAMP", new
        {
            id = walletId,
            status = Tables.Transac.PaymentStatus.Paid,
        });
    }
    public IEnumerable<Models.WalletBalance> GetWalletsBalance()
    {
        using var cnn = db.GetConnection();
        return cnn.Query<Models.WalletBalance>("SELECT WalletId, COALESCE(SUM(PaidValue),0) as Balance FROM Transac WHERE Status = 1 GROUP BY WalletId", null);
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
            SearchTransactionsDate.EffectiveDate => "[INV]",
            SearchTransactionsDate.DueDate => "DueDate",
            SearchTransactionsDate.PaymentDate => "PaymentDate",
            SearchTransactionsDate.Created => "Created",
            SearchTransactionsDate.Changed => "Changed",
            _ => throw new InvalidOperationException("Invalid date type"),
        };

        if (dateType == SearchTransactionsDate.PaymentDate)
        {
            add = "AND Status = @statusPaid";
        }
        string query = $"SELECT * FROM {nameof(Tables.Transac)} WHERE ({dateColumn} BETWEEN @start AND @end ) {add} ";

        if (dateType == SearchTransactionsDate.EffectiveDate)
        {
            string paidOnes = $"( {nameof(Tables.Transac.Status)} = @statusPaid AND {nameof(Tables.Transac.PaymentDate)} BETWEEN @start AND @end )";
            string unpaidOnes = $"( {nameof(Tables.Transac.Status)} = @statusUnpaid AND {nameof(Tables.Transac.DueDate)} BETWEEN @start AND @end )";

            query = $"SELECT * FROM {nameof(Tables.Transac)} WHERE ( {paidOnes} OR {unpaidOnes} )";
        }

        using var cnn = db.GetConnection();
        return cnn.Query<Tables.Transac>(query, new
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
            SearchTransactionsDate.EffectiveDate => "[INV]",
            SearchTransactionsDate.DueDate => "DueDate",
            SearchTransactionsDate.PaymentDate => "PaymentDate",
            SearchTransactionsDate.Created => "Created",
            SearchTransactionsDate.Changed => "Changed",
            _ => throw new InvalidOperationException("Invalid date type"),
        };
        if (dateType == SearchTransactionsDate.PaymentDate)
        {
            add = $"AND {nameof(Tables.Transac.Status)} = @statusPaid";
        }
        string query = $"SELECT * FROM {nameof(Tables.Transac)} WHERE {kindColumn} = @id AND ({dateColumn} BETWEEN @start AND @end ) {add} ";

        if (dateType == SearchTransactionsDate.EffectiveDate)
        {
            string paidOnes = $"( {nameof(Tables.Transac.Status)} = @statusPaid AND {nameof(Tables.Transac.PaymentDate)} BETWEEN @start AND @end )";
            string unpaidOnes = $"( {nameof(Tables.Transac.Status)} = @statusUnpaid AND {nameof(Tables.Transac.DueDate)} BETWEEN @start AND @end )";

            query = $"SELECT * FROM {nameof(Tables.Transac)} WHERE {kindColumn} = @id AND ( {paidOnes} OR {unpaidOnes} )";
        }

        using var cnn = db.GetConnection();
        return cnn.Query<Tables.Transac>(query, new
        {
            id,
            start,
            end,
            statusPaid = Tables.Transac.PaymentStatus.Paid,
            statusUnpaid = Tables.Transac.PaymentStatus.Unpaid,
        });
    }

    public long CreateUpdateTransaction(Tables.Transac tx) => createUpdateTransaction(tx, generateLog: true);
    private long createUpdateTransaction(Tables.Transac tx, bool generateLog)
    {
        using var cnn = db.GetConnection();
        var originalValue = tx.Id == 0 ? null : cnn.Get<Tables.Transac>(tx.Id);

        // check wallet
        var wallet = cnn.Get<Tables.Wallet>(tx.WalletId);
        if (wallet == null) throw new InvalidOperationException($"Invalid Wallet Id: {tx.WalletId}");

        // Check Category
        var category = cnn.Get<Tables.Category>(tx.CategoryId);
        if (category == null) throw new InvalidOperationException($"Invalid Category Id: {tx.CategoryId}");

        // Check signs
        if (tx.DueValue == 0) throw new InvalidOperationException($"'{nameof(Tables.Transac.DueValue)}' must not zero");
        var sign = category.IsExpense ? -1 : 1;
        tx.DueValue = Math.Abs(tx.PaidValue) * sign;
        tx.PaidValue = Math.Abs(tx.PaidValue) * sign;
        tx.RC_DueValue = Math.Abs(tx.RC_DueValue) * sign;
        tx.RC_PaidValue = Math.Abs(tx.RC_PaidValue) * sign;

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

        // Update dates
        tx.Changed = DateTime.UtcNow;
        if (tx.Id == 0) tx.Created = DateTime.UtcNow;
        else if (originalValue != null) tx.Created = originalValue.Created;

        tx.Id = (int)cnn.Insert(tx, OnConflict.Replace);

        if (generateLog) saveChangeLog(cnn, originalValue, tx);
        return tx.Id;
    }
    public void CreateWalletTransfer(long sourceWallet, long sourceCategory, long destinationWallet, long destinationCategory, string description, decimal value, DateTime date)
    {
        var categories = GetCategories().ToArray();
        var srcCat = categories.FirstOrDefault(o => o.Id == sourceCategory) ?? throw new ArgumentException("Invalid sourceCategory");
        var dstCat = categories.FirstOrDefault(o => o.Id == destinationCategory) ?? throw new ArgumentException("Invalid destinationCategory");

        if (!srcCat.IsExpense) throw new ArgumentException("sourceCategory must be 'IsExpense'");
        if (dstCat.IsExpense) throw new ArgumentException("destinationCategory must not be 'IsExpense'");

        var txPay = new Tables.Transac()
        {
            Id = 0,
            CategoryId = sourceCategory,
            Created = DateTime.UtcNow,
            Changed = DateTime.UtcNow,
            WalletId = sourceWallet,
            DueDate = date,
            DueValue = value,
            PaymentDate = date,
            PaidValue = value,
            Description = description,
            Status = Tables.Transac.PaymentStatus.Paid,
            Type = Tables.Transac.TransactionType.Simple, // Start as Simple
        };
        var txReceive = new Tables.Transac()
        {
            Id = 0,
            CategoryId = destinationCategory,
            Created = DateTime.UtcNow,
            Changed = DateTime.UtcNow,
            WalletId = destinationWallet,
            DueDate = date,
            DueValue = value,
            PaymentDate = date,
            PaidValue = value,
            Description = description,
            Status = Tables.Transac.PaymentStatus.Paid,
            Type = Tables.Transac.TransactionType.Simple, // Start as Simple
        };

        // save transactions
        txPay.Id = createUpdateTransaction(txPay, generateLog: false);
        txReceive.Id = createUpdateTransaction(txReceive, generateLog: false);
        // Update as Transfer
        using var cnn = db.GetConnection();
        cnn.Execute($"UPDATE {nameof(Tables.Transac)} SET Type = @type, TypeOtherId = @other WHERE Id = @id ", new
        {
            id = txPay.Id,
            type = Tables.Transac.TransactionType.WalletTransfer,
            other = txReceive.Id,
        });
        cnn.Execute($"UPDATE {nameof(Tables.Transac)} SET Type = @type, TypeOtherId = @other WHERE Id = @id ", new
        {
            id = txReceive.Id,
            type = Tables.Transac.TransactionType.WalletTransfer,
            other = txPay.Id,
        });
        // Generate Logs
        var first = cnn.Get<Tables.Transac>(txPay.Id) ?? throw new ArgumentException("Invalid transaction");
        var second = cnn.Get<Tables.Transac>(txReceive.Id) ?? throw new ArgumentException("Invalid second transaction");
        saveChangeLog(cnn, null, first);
        saveChangeLog(cnn, null, second);
    }

    public void UpdateWalletTransfer(long oneOfTransactions, decimal newValue, DateTime newDate) => updateWalletTransfer(oneOfTransactions,
            first =>
            {
                first.DueValue = first.PaidValue = newValue * Math.Sign(first.PaidValue);
                first.DueDate = first.PaymentDate = newDate;
            },
            second =>
            {
                second.DueValue = second.PaidValue = newValue * Math.Sign(second.PaidValue);
                second.DueDate = second.PaymentDate = newDate;
            });
    public void UpdateWalletTransfer(long oneOfTransactions, string description) => updateWalletTransfer(oneOfTransactions,
            first =>
            {
                first.Description = description;
            },
            second =>
            {
                second.Description = description;
            });
    public void ReverseWalletTransfer(long oneOfTransactions) => updateWalletTransfer(oneOfTransactions,
            first =>
            {
                first.Status = Tables.Transac.PaymentStatus.Reversed;
            },
            second =>
            {
                second.Status = Tables.Transac.PaymentStatus.Reversed;
            });
    void updateWalletTransfer(long oneOfTransactions, Action<Tables.Transac> firstUpdate, Action<Tables.Transac> secondUpdate)
    {
        using var cnn = db.GetConnection();
        var first = cnn.Get<Tables.Transac>(oneOfTransactions) ?? throw new ArgumentException("Invalid transaction");
        if (first.Type != Tables.Transac.TransactionType.WalletTransfer) throw new ArgumentException("Transaction is not a wallet transfer");
        var second = cnn.Get<Tables.Transac>(first.TypeOtherId) ?? throw new ArgumentException("Invalid second transaction");

        firstUpdate(first);
        first.Changed = DateTime.UtcNow;

        secondUpdate(second);
        second.Changed = DateTime.UtcNow;

        var oldFirst = cnn.Get<Tables.Transac>(first.Id);
        var oldSecond = cnn.Get<Tables.Transac>(second.Id);

        cnn.Insert(first, OnConflict.Replace);
        cnn.Insert(second, OnConflict.Replace);

        saveChangeLog(cnn, oldFirst, first);
        saveChangeLog(cnn, oldSecond, second);
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
    private bool saveChangeLog<T>(ISqliteConnection cnn, T? older, T newer)
    {
        var type = typeof(T);
        var diff = Helpers.ModelDiff(older, newer);

        var tableId = (long)type.GetProperties().Where(o => o.Name == "Id").First().GetValue(newer);

        var logId = cnn.Insert(new Tables.ChangeLog
        {
            Id = 0,
            Event = DateTime.UtcNow,
            TableName = type.FullName,
            TableId = tableId,
        });
        var records = diff.Keys.Select(o => new Tables.ChangeLogItem
        {
            Id = 0,
            LogId = logId,
            FieldName = o,
            OldValue = diff[o].Item1,
            NewValue = diff[o].Item2,
        })
        .ToArray();
        cnn.BulkInsert(records);

        notify(type.FullName, older == null ? ManagerNotificationEventArgs.EventNotificationKind.New : ManagerNotificationEventArgs.EventNotificationKind.Update, tableId);
        return records.Length > 0;
    }

    private void notify(string tableName, ManagerNotificationEventArgs.EventNotificationKind eventNotificationKind, long id)
    {
        if (EventNotifier == null) return;

        var tableEnum = tableName switch
        {
            "Simple.Finance.Tables.Category" => ManagerNotificationEventArgs.EventNotificationType.Category,
            "Simple.Finance.Tables.Person" => ManagerNotificationEventArgs.EventNotificationType.Person,
            "Simple.Finance.Tables.Transac" => ManagerNotificationEventArgs.EventNotificationType.Transaction,
            "Simple.Finance.Tables.Wallet" => ManagerNotificationEventArgs.EventNotificationType.Wallet,
            _ => throw new NotImplementedException()
        };

        EventNotifier.Invoke(this, new ManagerNotificationEventArgs
        {
            Kind = eventNotificationKind,
            Id = id,
        });
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
        /// <summary>
        /// Due date for Unpaid and PaymentDate for paid ones
        /// </summary>
        EffectiveDate,
    }

    #endregion

}
