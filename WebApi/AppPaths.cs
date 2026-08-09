namespace Simple.Finance.WebApi;

using System;
using System.IO;

/// <summary>
/// Every path the application uses, all rooted at the application folder.
/// Nothing is written outside of it
/// </summary>
public static class AppPaths
{
    /// <summary>
    /// Application folder, where the assemblies live
    /// </summary>
    public static string Root { get; } = AppContext.BaseDirectory;
    /// <summary>
    /// Local data root: [app]/data
    /// </summary>
    public static string Data { get; } = Path.Combine(Root, "data");
    /// <summary>
    /// Log folder: [app]/data/log
    /// </summary>
    public static string LogFolder { get; } = Path.Combine(Data, "log");
    /// <summary>
    /// Serilog base file, the daily 'yyyyMMdd' is appended by the rolling policy: [app]/data/log/LogyyyyMMdd.log
    /// </summary>
    public static string LogFile { get; } = Path.Combine(LogFolder, "Log.log");
    /// <summary>
    /// Management database, holds accounts and preferences: [app]/data/db.sqlite
    /// </summary>
    public static string ManagementDbFile { get; } = Path.Combine(Data, "db.sqlite");
    /// <summary>
    /// Accounts folder: [app]/data/users
    /// </summary>
    public static string UsersFolder { get; } = Path.Combine(Data, "users");
    /// <summary>
    /// Backups folder, one subfolder per account: [app]/data/users/bkp
    /// </summary>
    public static string BackupFolder { get; } = Path.Combine(UsersFolder, "bkp");

    /// <summary>
    /// Finance database of a single account: [app]/data/users/db_{key}.sqlite
    /// </summary>
    public static string UserDbFile(Guid accountKey) => Path.Combine(UsersFolder, $"db_{accountKey:D}.sqlite");
    /// <summary>
    /// Backup folder of a single account: [app]/data/users/bkp/{key}
    /// </summary>
    public static string UserBackupFolder(Guid accountKey) => Path.Combine(BackupFolder, accountKey.ToString("D"));

    /// <summary>
    /// Creates the fixed folder structure, must run before the logger is created
    /// </summary>
    public static void EnsureFolders()
    {
        Directory.CreateDirectory(LogFolder);
        Directory.CreateDirectory(BackupFolder); // Creates 'users' on the way
    }
}
