namespace Simple.Finance.Tables;

using Simple.DatabaseWrapper.Attributes;
using System;

public record ScenarioItem
{
    [PrimaryKey]
    public long Id { get; set; }
    [Index("ixScenarioItem_ScenarioId")]
    public long ScenarioId { get; set; }

    public long WalletId { get; set; }
    public long CategoryId { get; set; }
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string? ExternalIdentifier { get; set; }
}
