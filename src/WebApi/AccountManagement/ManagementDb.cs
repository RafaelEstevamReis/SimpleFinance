namespace Simple.Finance.WebApi.AccountManagement;

using Simple.Sqlite;
using System;
using System.Linq;

/// <summary>
/// The management database at [app]/data/db.sqlite: accounts and their preferences.
/// Finance data never lands here, each account has its own database
/// </summary>
public class ManagementDb
{
    private readonly ConnectionFactory db;

    public ManagementDb()
    {
        db = ConnectionFactory.FromFile(AppPaths.ManagementDbFile);
    }

    /// <summary>
    /// Creates and migrates the schema, must run once on startup
    /// </summary>
    public void Initialize()
    {
        using var cnn = db.GetConnection();
        cnn.CreateTables()
           .Add<Account>()
           .Add<AccountPreference>()
           .Commit();
    }

    /// <summary>
    /// Creates a new account with a brand new Key. The Key is returned only here,
    /// there is no way to recover it later
    /// </summary>
    public Account CreateAccount(string name)
    {
        var account = new Account
        {
            Key = Guid.NewGuid(),
            Name = name,
            Created = DateTime.UtcNow,
            LastAccess = DateTime.UtcNow,
            IsEnabled = true,
        };

        using var cnn = db.GetConnection();
        cnn.Insert(account);

        return account;
    }

    /// <summary>
    /// Gets an account by its Key, null when there is no such account
    /// </summary>
    public Account? GetAccount(Guid key)
    {
        using var cnn = db.GetConnection();
        return cnn.Get<Account>(key);
    }

    /// <summary>
    /// Stamps <see cref="Account.LastAccess"/>, called on every authenticated request
    /// </summary>
    public void TouchLastAccess(Guid key)
    {
        using var cnn = db.GetConnection();
        cnn.Execute($"UPDATE {nameof(Account)} SET {nameof(Account.LastAccess)} = @now WHERE {nameof(Account.Key)} = @key", new
        {
            now = DateTime.UtcNow,
            key,
        });
    }

    /// <summary>
    /// All preferences of an account
    /// </summary>
    public AccountPreference[] GetPreferences(Guid accountKey)
    {
        using var cnn = db.GetConnection();
        return [.. cnn.GetWhere<AccountPreference>(nameof(AccountPreference.AccountKey), accountKey)];
    }

    /// <summary>
    /// Creates or replaces a single preference of an account.
    /// The unique key makes this a single statement, with no read-then-write window
    /// </summary>
    public void SetPreference(Guid accountKey, string name, string value)
    {
        using var cnn = db.GetConnection();
        cnn.Insert(new AccountPreference
        {
            Id = 0,
            AccountKey = accountKey,
            Name = name,
            Value = value,
        }, OnConflict.Replace);
    }

    /// <summary>
    /// Removes a single preference, returns false when there was nothing to remove
    /// </summary>
    public bool RemovePreference(Guid accountKey, string name)
    {
        using var cnn = db.GetConnection();
        var affected = cnn.Execute(
            $"DELETE FROM {nameof(AccountPreference)} WHERE {nameof(AccountPreference.Key)} = @key",
            new { key = AccountPreference.KeyOf(accountKey, name) });

        return affected > 0;
    }

}
