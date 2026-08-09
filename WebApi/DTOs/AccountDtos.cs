namespace Simple.Finance.WebApi.DTOs;

using System;

/// <summary>
/// Data to create a new account
/// </summary>
public record CreateAccountRequest
{
    /// <summary>
    /// Free text to tell the accounts apart, optional
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// A brand new account. The Key is shown here and nowhere else, store it
/// </summary>
public record CreateAccountResponse
{
    /// <summary>
    /// The account Key, send it on every request to authenticate
    /// </summary>
    public Guid Key { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Created { get; set; }
}

/// <summary>
/// The account behind the current Key
/// </summary>
public record AccountResponse
{
    public Guid Key { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime LastAccess { get; set; }
}

/// <summary>
/// State of the finance database of an account
/// </summary>
public record AccountDatabaseResponse
{
    /// <summary>
    /// File name inside data/users, never the full path
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    /// <summary>
    /// Wallets already registered, proves the schema is ready
    /// </summary>
    public int WalletCount { get; set; }
    /// <summary>
    /// Newest backup file in data/users/bkp, null when none was taken yet
    /// </summary>
    public string? LastBackup { get; set; }
    public DateTime? LastBackupUtc { get; set; }
}
