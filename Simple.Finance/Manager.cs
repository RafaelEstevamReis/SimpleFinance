namespace Simple.Finance;

using Simple.Finance.Helpers;
using Simple.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

public class Manager
{
    /// <summary>
    /// Internal Sqlite factory
    /// </summary>
    protected readonly ConnectionFactory db;
    private readonly string dbFile;

    /// <summary>
    /// Triggers an event when a item is updated in this instance
    /// </summary>
    public event EventHandler<ManagerNotificationEventArgs>? EventNotifier;
    /// <summary>
    /// Sets current `ExternalId` for all ChangeLogs insertions
    /// Can be used to set "User who made the change"
    /// </summary>
    public long EventLogCurrentExternalId { get; set; } = 0;

    public Manager(string dbFile)
    {
        db = ConnectionFactory.FromFile(dbFile);
        this.dbFile = dbFile;
    }
    private Manager(ConnectionFactory db, string dbFile)
    {
        this.db = db;
        this.dbFile = dbFile;
    }
    public static Manager FromDatabase(ConnectionFactory db) => new Manager(db, string.Empty);

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
            if (string.IsNullOrEmpty(dbFile)) throw new ArgumentException(nameof(createBackup), "createBackup is not compatible with external databases");
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

        InternalInitialize(cnn);
    }
    /// <summary>
    /// Init function free for derived classes to use,
    /// it runs after migrations and does nothing
    /// </summary>
    protected virtual void InternalInitialize(ISqliteConnection cnn) { }

    static void compress(string dbFile, string destFile)
    {
        var fiOrg = new FileInfo(dbFile);
        if (!fiOrg.Exists) return; // First run

        var fiDest = new FileInfo(destFile);
        if (!fiDest.Directory.Exists) fiDest.Directory.Create();
        var zipName = fiDest.FullName + ".gz";

        using FileStream originalFileStream = File.Open(fiOrg.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
        return cnn.ExecuteScalar<decimal>($"SELECT COALESCE(SUM(PaidValue),0) FROM {nameof(Tables.Transac)} WHERE WalletId = @id AND Status = @status AND PaymentDate <= CURRENT_TIMESTAMP", new
        {
            id = walletId,
            status = Tables.Transac.PaymentStatus.Paid,
        });
    }
    public IEnumerable<Models.WalletBalance> GetWalletsBalance()
    {
        using var cnn = db.GetConnection();
        return cnn.Query<Models.WalletBalance>(
$@"SELECT WalletId, COALESCE(SUM(PaidValue),0) as Balance 
  FROM {nameof(Tables.Transac)} 
  WHERE Status = @statusPaid 
GROUP BY WalletId", new
{
    statusPaid = Tables.Transac.PaymentStatus.Paid,
});
    }
    public IEnumerable<Models.WalletBalance> GetWalletsBalance(DateTime atDate)
    {
        using var cnn = db.GetConnection();
        return cnn.Query<Models.WalletBalance>(
$@"SELECT WalletId, COALESCE(SUM(CASE WHEN Status = @statusPaid THEN  PaidValue ELSE DueValue END),0) as Balance 
  FROM {nameof(Tables.Transac)} 
  WHERE (Status = @statusPaid AND PaymentDate < @date) 
     OR (Status = @statusUnpaid AND DueDate <= @date AND DueDate > CURRENT_TIMESTAMP ) 
GROUP BY WalletId", new
{
    date = atDate,
    statusPaid = Tables.Transac.PaymentStatus.Paid,
    statusUnpaid = Tables.Transac.PaymentStatus.Unpaid,
});
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
        // Indexes will be bigger than the table itself and the same impact as tablescan
        // just get all and filter later
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

    public Tables.Transac? GetTransactionById(long id)
    {
        using var cnn = db.GetConnection();
        return cnn.Get<Tables.Transac>(id);
    }
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
            string unpaidOnes = $"( {nameof(Tables.Transac.Status)} <> @statusPaid AND {nameof(Tables.Transac.DueDate)} BETWEEN @start AND @end )";

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
            string unpaidOnes = $"( {nameof(Tables.Transac.Status)} <> @statusPaid AND {nameof(Tables.Transac.DueDate)} BETWEEN @start AND @end )";

            query = $"SELECT * FROM {nameof(Tables.Transac)} WHERE {kindColumn} = @id AND ( {paidOnes} OR {unpaidOnes} )";
        }

        using var cnn = db.GetConnection();
        return cnn.Query<Tables.Transac>(query, new
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
        return createUpdateTransaction(cnn, tx, generateLog: true, generateNotification: true);
    }
    public IEnumerable<long> CreateUpdateBulkTransaction(IEnumerable<Tables.Transac> txs)
    {
        List<long> lst = [];
        using (var cnn = db.GetConnection())
        {
            foreach (var tx in txs)
            {
                var id = createUpdateTransaction(cnn, tx, generateLog: true, generateNotification: false);
                lst.Add(id);
            }
        }
        // Notify All after closing the connection
        var tableName = getTableName(typeof(Tables.Transac));
        foreach (var tx in txs)
        {
            triggerNotification(tableName, ManagerNotificationEventArgs.EventNotificationAction.Update, tx.Id);
        }
        return lst;
    }
    private long createUpdateTransaction(ISqliteConnection cnn, Tables.Transac tx, bool generateLog, bool generateNotification)
    {
        if (!generateLog && generateNotification) Debug.Fail("Cannot notify without log");

        var originalValue = tx.Id == 0 ? null : cnn.Get<Tables.Transac>(tx.Id);

        // check wallet
        var wallet = cnn.Get<Tables.Wallet>(tx.WalletId);
        if (wallet == null) throw new InvalidOperationException($"Invalid Wallet Id: {tx.WalletId}");

        // Check Category
        var category = cnn.Get<Tables.Category>(tx.CategoryId);
        if (tx.CategoryId != 0 && category == null) throw new InvalidOperationException($"Invalid Category Id: {tx.CategoryId}");

        // Check signs
        if (tx.DueValue == 0) throw new InvalidOperationException($"'{nameof(Tables.Transac.DueValue)}' must not zero");
        var sign = Math.Sign(tx.DueValue);
        if (category != null)
        {
            sign = category.IsExpense ? -1 : 1;
        }

        tx.DueValue = Math.Abs(tx.DueValue) * sign;
        tx.PaidValue = Math.Abs(tx.PaidValue) * sign;
        tx.RC_DueValue = Math.Abs(tx.RC_DueValue) * sign;
        tx.RC_PaidValue = Math.Abs(tx.RC_PaidValue) * sign;

        // Check Currency
        tx.PaymentCurrency = tx.PaymentCurrency.ToUpper();
        if (!string.IsNullOrEmpty(wallet.BaseCurrency)
            && !string.IsNullOrEmpty(tx.PaymentCurrency))
        {
            if (wallet.BaseCurrency != tx.PaymentCurrency)
            {
                throw new InvalidOperationException($"PaymentCurrency must be the same as Wallet BaseCurrency");
            }
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

        // Update dates
        tx.Changed = DateTime.UtcNow;
        if (tx.Id == 0) tx.Created = DateTime.UtcNow;
        else if (originalValue != null) tx.Created = originalValue.Created;

        tx.Id = (int)cnn.Insert(tx, OnConflict.Replace);

        if (generateLog)
        {
            saveChangeLog(cnn, originalValue, tx, generateNotification);
        }
        return tx.Id;
    }

    [Obsolete("Use separated dates instead", error: false)]
    public void CreateWalletTransfer(long sourceWallet, long sourceCategory, long destinationWallet, long destinationCategory, string description, decimal value, DateTime date, bool paid)
        => CreateWalletTransfer(sourceWallet, sourceCategory, destinationWallet, destinationCategory, description, value, date, date, paid, null);

    public (long payId, long receiveId) CreateWalletTransfer(long sourceWallet, long sourceCategory, long destinationWallet, long destinationCategory, string description, decimal value, DateTime dueDate, DateTime paymentDate, bool paid, string? paymentDetails)
    {
        if (sourceCategory != 0 || destinationCategory != 0)
        {
            var categories = GetCategories().ToArray();

            var srcCat = categories.FirstOrDefault(o => o.Id == sourceCategory) ?? throw new ArgumentException("Invalid sourceCategory");
            var dstCat = categories.FirstOrDefault(o => o.Id == destinationCategory) ?? throw new ArgumentException("Invalid destinationCategory");

            if (!srcCat.IsExpense) throw new ArgumentException("sourceCategory must be 'IsExpense'");
            if (dstCat.IsExpense) throw new ArgumentException("destinationCategory must not be 'IsExpense'");
        }

        var txPay = new Tables.Transac()
        {
            Id = 0,
            CategoryId = sourceCategory,
            Created = DateTime.UtcNow,
            Changed = DateTime.UtcNow,
            WalletId = sourceWallet,
            DueDate = dueDate,
            DueValue = -value,
            PaymentDate = paymentDate,
            PaidValue = -value,
            Description = description,
            PaymentDetails = paymentDetails,
            Status = paid ? Tables.Transac.PaymentStatus.Paid : Tables.Transac.PaymentStatus.Unpaid,
            Type = Tables.Transac.TransactionType.Simple, // Start as Simple
        };
        var txReceive = new Tables.Transac()
        {
            Id = 0,
            CategoryId = destinationCategory,
            Created = DateTime.UtcNow,
            Changed = DateTime.UtcNow,
            WalletId = destinationWallet,
            DueDate = dueDate,
            DueValue = value,
            PaymentDate = paymentDate,
            PaidValue = value,
            Description = description,
            PaymentDetails = paymentDetails,
            Status = paid ? Tables.Transac.PaymentStatus.Paid : Tables.Transac.PaymentStatus.Unpaid,
            Type = Tables.Transac.TransactionType.Simple, // Start as Simple
        };

        using var cnn = db.GetConnection();
        // save transactions
        txPay.Id = createUpdateTransaction(cnn, txPay, generateLog: false, generateNotification: false);
        txReceive.Id = createUpdateTransaction(cnn, txReceive, generateLog: false, generateNotification: false);
        // Update as Transfer
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

        return (txPay.Id, txReceive.Id);
    }

    [Obsolete("Use separated dates instead", error: false)]
    public void UpdateWalletTransfer(long oneOfTransactions, decimal newValue, DateTime newDate, string description, Tables.Transac.PaymentStatus status)
        => UpdateWalletTransfer(oneOfTransactions, newValue, newValue, newDate, newDate, description, null, status);

    public void UpdateWalletTransfer(long oneOfTransactions, decimal newDueValue, decimal newPaidValue, DateTime newDueDate, DateTime newPaymentDate, string description, string? paymentDetails, Tables.Transac.PaymentStatus status)
    {
        updateWalletTransfer(oneOfTransactions,
            expenseUpdate =>
            {
                expenseUpdate.DueValue = newDueValue * (-1);
                expenseUpdate.PaidValue = newPaidValue * (-1);
                expenseUpdate.DueDate = newDueDate;
                expenseUpdate.PaymentDate = newPaymentDate;
                expenseUpdate.Description = description;
                expenseUpdate.Status = status;
                expenseUpdate.PaymentDetails = paymentDetails;
            },
            incomeUpdate =>
            {
                incomeUpdate.DueValue = newDueValue * (+1);
                incomeUpdate.PaidValue = newPaidValue * (+1);
                incomeUpdate.DueDate = newDueDate;
                incomeUpdate.PaymentDate = newPaymentDate;
                incomeUpdate.Description = description;
                incomeUpdate.Status = status;
                incomeUpdate.PaymentDetails = paymentDetails;
            });
    }
    void updateWalletTransfer(long oneOfTransactions, Action<Tables.Transac> expenseUpdate, Action<Tables.Transac> incomeUpdate)
    {
        using var cnn = db.GetConnection();

        var first = cnn.Get<Tables.Transac>(oneOfTransactions) ?? throw new ArgumentException("Invalid transaction");
        if (first.Type != Tables.Transac.TransactionType.WalletTransfer) throw new ArgumentException("Transaction is not a wallet transfer");
        var second = cnn.Get<Tables.Transac>(first.TypeOtherId) ?? throw new ArgumentException("Invalid second transaction");

        expenseUpdate(first.DueValue < 0 ? first : second);
        incomeUpdate(first.DueValue > 0 ? first : second);

        first.Changed = DateTime.UtcNow;
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

    private const string sqlLogSelect = $@"
        SELECT 
            cl.Id          AS LogId,
            cl.Event       AS Event,
            cl.TableName   AS TableName,
            cl.TableId     AS TableId,
            cl.ExternalId  AS ExternalId,

            cli.Id         AS LogItemId,
            cli.FieldName  AS FieldName,
            cli.OldValue   AS OldValue,
            cli.NewValue   AS NewValue
        FROM {nameof(Tables.ChangeLog)} cl
        INNER JOIN {nameof(Tables.ChangeLogItem)} cli ON cli.LogId = cl.Id";
    private const string sqlLogOrder = @"
        ORDER BY cl.Event, cli.Id, cli.FieldName";

    public IEnumerable<Tables.TableLogRegistry> GetLogs(DateTime start, DateTime end)
    {
        const string sql = sqlLogSelect + @"
        WHERE cl.Event BETWEEN @start AND @end" + sqlLogOrder;

        using var cnn = db.GetConnection();
        return cnn.Query<Tables.TableLogRegistry>(sql, new { start, end });
    }
    public IEnumerable<Tables.TableLogRegistry> GetLogs(DateTime start, DateTime end, long externalId)
    {
        const string sql = sqlLogSelect + @"
        WHERE cl.Event BETWEEN @start AND @end AND cl.ExternalId = @externalId" + sqlLogOrder;

        using var cnn = db.GetConnection();
        return cnn.Query<Tables.TableLogRegistry>(sql, new { start, end, externalId });
    }
    public IEnumerable<Tables.TableLogRegistry> GetLogs<T>(long tableId)
    {
        if (tableId <= 0) return [];
        var tableName = getTableName(typeof(T));

        const string sql = sqlLogSelect + @"
        WHERE cl.TableName = @tableName AND cl.TableId = @tableId" + sqlLogOrder;

        using var cnn = db.GetConnection();
        return cnn.Query<Tables.TableLogRegistry>(sql, new { tableName, tableId });
    }
    protected bool saveChangeLog<T>(ISqliteConnection cnn, T? older, T newer, bool notify = true)
    {
        var type = typeof(T);
        var diff = ModelHelpers.ModelDiff(older, newer);
        var tableId = (long)type.GetProperties().Where(o => o.Name == "Id").First().GetValue(newer);
        var tableName = getTableName(type);

        var logId = cnn.Insert(new Tables.ChangeLog
        {
            Id = 0,
            Event = DateTime.UtcNow,
            TableName = tableName,
            TableId = tableId,
            ExternalId = EventLogCurrentExternalId,
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

        if (notify) triggerNotification(tableName, older == null ? ManagerNotificationEventArgs.EventNotificationAction.New : ManagerNotificationEventArgs.EventNotificationAction.Update, tableId);
        return records.Length > 0;
    }
    protected static string getTableName(Type table)
    {
        var tableName = table.FullName.Split('.')[^1];
        return tableName;
    }

    #endregion

    #region Notification

    /// <summary>
    /// Maps a table name, as stored on <see cref="Tables.ChangeLog.TableName"/>, to its notification item.
    /// Built from the types themselves so renaming a table cannot silently break the routing
    /// </summary>
    private static readonly Dictionary<string, ManagerNotificationEventArgs.EventNotificationItem> notificationItems = new()
    {
        [getTableName(typeof(Tables.Category))] = ManagerNotificationEventArgs.EventNotificationItem.Category,
        [getTableName(typeof(Tables.Person))] = ManagerNotificationEventArgs.EventNotificationItem.Person,
        [getTableName(typeof(Tables.Transac))] = ManagerNotificationEventArgs.EventNotificationItem.Transaction,
        [getTableName(typeof(Tables.Wallet))] = ManagerNotificationEventArgs.EventNotificationItem.Wallet,
    };

    protected void triggerNotification(string tableName, ManagerNotificationEventArgs.EventNotificationAction eventNotificationAction, long id)
    {
        if (EventNotifier == null) return;

        if (!notificationItems.TryGetValue(tableName, out var tableEnum))
        {
            tableEnum = ManagerNotificationEventArgs.EventNotificationItem.Other;
        }

        EventNotifier.Invoke(this, new ManagerNotificationEventArgs
        {
            Item = tableEnum,
            Action = eventNotificationAction,
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
