namespace Simple.Finance.WebApi.AccountManagement;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;

/// <summary>
/// Keeps one live <see cref="Manager"/> per account.
/// Entering the cache is what initializes (and backs up) that account's database
/// </summary>
public class ManagerCache(IMemoryCache cache, ILogger<ManagerCache> logger)
{
    /// <summary>
    /// How long an idle account keeps its Manager alive
    /// </summary>
    public static readonly TimeSpan IdleExpiration = TimeSpan.FromMinutes(30);

    // Only guards creation: two requests of the same account must not both
    // initialize and back up the same file. Usage itself is not serialized
    private readonly Lock createLock = new();

    /// <summary>
    /// The Manager of an account, creating and initializing it on first use
    /// </summary>
    public Manager GetFor(Guid accountKey)
    {
        var cacheKey = keyOf(accountKey);
        if (cache.TryGetValue<Manager>(cacheKey, out var cached) && cached is not null) return cached;

        lock (createLock)
        {
            if (cache.TryGetValue(cacheKey, out cached) && cached is not null) return cached;

            var manager = create(accountKey);

            var options = new MemoryCacheEntryOptions { SlidingExpiration = IdleExpiration };
            options.RegisterPostEvictionCallback((_, _, reason, _)
                => logger.LogInformation("Released Manager of {AccountKey} ({Reason})", accountKey, reason));

            return cache.Set(cacheKey, manager, options);
        }
    }

    /// <summary>
    /// Backup file of the current day for an account, '.gz' is appended by the Manager
    /// </summary>
    public static string BackupNameFor(Guid accountKey, DateTime utcDay)
        => Path.Combine(AppPaths.UserBackupFolder(accountKey), utcDay.ToString("yyyyMMdd"));

    private Manager create(Guid accountKey)
    {
        var dbFile = AppPaths.UserDbFile(accountKey);
        var backupName = BackupNameFor(accountKey, DateTime.UtcNow);
        // One backup per day per account: the first Manager of the day takes it,
        // so a later session cannot overwrite the good copy with a damaged one
        var backupToday = !File.Exists(backupName + ".gz");

        var manager = new Manager(dbFile);
        manager.Initialize(createBackup: backupToday, backupName: backupName);

        logger.LogInformation("Initialized Manager of {AccountKey} (backup: {Backup})", accountKey, backupToday ? backupName + ".gz" : "already done today");
        return manager;
    }

    private static string keyOf(Guid accountKey) => $"manager:{accountKey:D}";
}
