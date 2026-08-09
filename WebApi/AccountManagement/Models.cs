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
    [Index("ixAccountPreference_Account", 1)]
    public Guid AccountKey { get; set; }
    [Index("ixAccountPreference_Account", 2)]
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
