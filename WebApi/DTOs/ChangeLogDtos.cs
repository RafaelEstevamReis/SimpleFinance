namespace Simple.Finance.WebApi.DTOs;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Tables that keep a change history
/// </summary>
public enum ChangeLogTable
{
    Wallet,
    Category,
    Person,
    Transaction,
}

/// <summary>
/// One field that changed on an event
/// </summary>
public record ChangeLogFieldChange
{
    public string FieldName { get; set; } = string.Empty;
    /// <summary>
    /// Value before the change, exactly as stored.
    /// <see cref="ChangeLogEntryResponse.NoValue"/> means there was none
    /// </summary>
    public string OldValue { get; set; } = string.Empty;
    /// <summary>
    /// Value after the change, exactly as stored
    /// </summary>
    public string NewValue { get; set; } = string.Empty;
}

/// <summary>
/// A single change event: one write, with every field it touched
/// </summary>
public record ChangeLogEntryResponse
{
    /// <summary>
    /// Marker the library stores in place of a null value
    /// </summary>
    public const string NoValue = "[NL]";

    public long LogId { get; set; }
    /// <summary>
    /// When the write happened, UTC
    /// </summary>
    public DateTime Event { get; set; }
    /// <summary>
    /// Table that was written, as stored by the library ('Wallet', 'Category', 'Person', 'Transac')
    /// </summary>
    public string Table { get; set; } = string.Empty;
    /// <summary>
    /// Id of the changed row
    /// </summary>
    public long TableId { get; set; }
    /// <summary>
    /// Author of the change. Always 0 unless something sets the Manager's EventLogCurrentExternalId
    /// </summary>
    public long ExternalId { get; set; }
    /// <summary>
    /// True when the row was created by this event, meaning it had no previous value
    /// </summary>
    public bool IsCreation { get; set; }

    public ChangeLogFieldChange[] Changes { get; set; } = [];

    /// <summary>
    /// Folds the flat join the library returns into one entry per event
    /// </summary>
    public static ChangeLogEntryResponse[] FromRows(IEnumerable<Tables.TableLogRegistry> rows)
        => rows.GroupBy(o => o.LogId)
               .Select(group =>
               {
                   var first = group.First();
                   return new ChangeLogEntryResponse
                   {
                       LogId = group.Key,
                       Event = first.Event,
                       Table = first.TableName,
                       TableId = first.TableId,
                       ExternalId = first.ExternalId,
                       IsCreation = group.All(o => o.OldValue == NoValue),
                       Changes = group.Select(o => new ChangeLogFieldChange
                       {
                           FieldName = o.FieldName,
                           OldValue = o.OldValue,
                           NewValue = o.NewValue,
                       }).ToArray(),
                   };
               })
               .OrderBy(o => o.Event)
               .ThenBy(o => o.LogId)
               .ToArray();
}
