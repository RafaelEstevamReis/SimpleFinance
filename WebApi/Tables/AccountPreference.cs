namespace Simple.Finance.WebApi.Tables;

using Simple.DatabaseWrapper.Attributes;
using System;

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
