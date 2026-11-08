namespace Simple.Finance.WebApi.DTOs;

using System;

/// <summary>
/// Scenario to create or update: a named set of hypothetical movements.
/// Scenarios never touch the real transactions
/// </summary>
public record ScenarioRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    /// <summary>
    /// Active scenarios are composed together by the projection, they are not exclusive
    /// </summary>
    public bool IsActive { get; set; }

    public Tables.Scenario ToTable(long id) => new()
    {
        Id = id,
        Name = Name,
        Description = Description,
        IsActive = IsActive,
    };
}

public record ScenarioResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public static ScenarioResponse From(Tables.Scenario scenario) => new()
    {
        Id = scenario.Id,
        Name = scenario.Name,
        Description = scenario.Description,
        IsActive = scenario.IsActive,
    };
}

/// <summary>
/// Item of a scenario: one hypothetical movement on one wallet, at one date.
/// The sign comes from the category, so positive values are fine
/// </summary>
public record ScenarioItemRequest
{
    public long WalletId { get; set; }
    /// <summary>
    /// 0 leaves the item uncategorised, and then the sent sign is kept
    /// </summary>
    public long CategoryId { get; set; }
    public DateTime Date { get; set; }
    /// <summary>
    /// Must not be zero. The category decides whether it is stored negative or positive
    /// </summary>
    public decimal Value { get; set; }
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Disabled items stay on the scenario but are ignored by the projection
    /// </summary>
    public bool IsEnabled { get; set; }
    /// <summary>
    /// Free slot for external applications: the library never reads it,
    /// it is stored and returned as sent
    /// </summary>
    public string? ExternalIdentifier { get; set; }

    public Tables.ScenarioItem ToTable(long id, long scenarioId) => new()
    {
        Id = id,
        ScenarioId = scenarioId,
        WalletId = WalletId,
        CategoryId = CategoryId,
        Date = Date,
        Value = Value,
        Name = Name,
        IsEnabled = IsEnabled,
        ExternalIdentifier = ExternalIdentifier,
    };
}

/// <summary>
/// Item of a bulk upsert, where each entry chooses between creating and updating
/// </summary>
public record ScenarioItemBulkRequest : ScenarioItemRequest
{
    /// <summary>
    /// 0 creates a new item, an existing id replaces that item
    /// </summary>
    public long Id { get; set; }
}

public record ScenarioItemResponse
{
    public long Id { get; set; }
    public long ScenarioId { get; set; }
    public long WalletId { get; set; }
    public long CategoryId { get; set; }
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string? ExternalIdentifier { get; set; }

    public static ScenarioItemResponse From(Tables.ScenarioItem item) => new()
    {
        Id = item.Id,
        ScenarioId = item.ScenarioId,
        WalletId = item.WalletId,
        CategoryId = item.CategoryId,
        Date = item.Date,
        Value = item.Value,
        Name = item.Name,
        IsEnabled = item.IsEnabled,
        ExternalIdentifier = item.ExternalIdentifier,
    };
}

/// <summary>
/// Mass toggle: the ids to write and the value to write on them.
/// Only the flag is written, every other field of those rows is left alone
/// </summary>
public record ScenarioToggleRequest
{
    public long[] Ids { get; set; } = [];
    /// <summary>
    /// The value the flag takes: active for scenarios, enabled for items
    /// </summary>
    public bool State { get; set; }
}
