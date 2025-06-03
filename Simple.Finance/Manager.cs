namespace Simple.Finance;

using Simple.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

public class Manager
{
    private readonly ConnectionFactory db;

    public Manager(string dbFile)
    {
        db = ConnectionFactory.FromFile(dbFile);
    }

    public void Initialize()
    {
        using var cnn = db.GetConnection();
        cnn.CreateTables()
           .Add<Tables.ChangeLog>()
           .Add<Tables.Category>()
           .Add<Tables.Wallet>()
           .Add<Tables.Person>()
           .Add<Tables.Transaction>()
           .Commit();
    }

    #region Wallets

    public IEnumerable< Tables.Wallet> GetWallets()
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

    public long CreateUpdatePerson(Tables.Person person)
    {
        using var cnn = db.GetConnection();
        var originalValue = person.Id == 0 ? null : cnn.Get<Tables.Person>(person.Id);

        person.Id = (int)cnn.Insert(person, OnConflict.Replace);

        saveChangeLog(cnn, originalValue, person);
        return person.Id;
    }
    public IEnumerable<Tables.Person> GetAllPersons()
    {
        using var cnn = db.GetConnection();
        return cnn.GetAll<Tables.Person>();
    }

    #endregion

    #region Transactions



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
            Table = type.FullName,
            Field = o,
            OldValue = diff[o].Item1,
            NewValue = diff[o].Item2,
        })
        .ToArray();

        cnn.BulkInsert(records);

        return records.Length > 0;
    }
    #endregion
}
