namespace Simple.Finance.WebApi.AccountManagement;

using Simple.DatabaseWrapper.Attributes;
using System;

/// <summary>
/// An account of this service. The Key is the credential AND the name of the
/// finance database file, whoever holds it has full access to that database
/// </summary>
public record Account
{
    [PrimaryKey]
    public Guid Key { get; set; }
    /// <summary>
    /// Free text to tell the accounts apart, not a credential
    /// </summary>
    public string Name { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime LastAccess { get; set; }
    /// <summary>
    /// Disabled accounts are rejected by the authentication, the database file is kept
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// A single preference of an account, kept on the management database
/// so it survives independently of the finance data
/// </summary>
public record AccountPreference
{
    [PrimaryKey]
    public long Id { get; set; }
    /// <summary>
    /// '{AccountKey}/{Name}'. Composed by <see cref="ManagementDb"/> and by nobody else,
    /// it exists only to carry the uniqueness of the pair on a single column.
    /// The '/' is a safe separator because a preference name can never contain one
    /// </summary>
    [Unique]
    public string Key { get; set; } = string.Empty;
    [Index("ixAccountPreference_Account")]
    public Guid AccountKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
