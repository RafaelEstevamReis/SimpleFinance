namespace Simple.Finance.Tables;

using Simple.DatabaseWrapper.Attributes;
using System;

public record ChangeLog
{
    [PrimaryKey]
    public long Id { get; set; }
    [Index("ixChangeLog_Event")]
    public DateTime Event { get; set; }
    [Index("ixChangeLog_Table", 0)]
    public string TableName { get; set; } = string.Empty;
    [Index("ixChangeLog_Table", 1)]
    public long TableId { get; set; }
    public long ExternalId { get; set; }
}
public record ChangeLogItem
{
    [PrimaryKey]
    public long Id { get; set; }
    [Index("ixChangeLogItem_LogId")]
    public long LogId { get; set; }

    public string FieldName { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
}

public record TableLogRegistry
{
    public long LogId { get; set; }
    public DateTime Event { get; set; }
    public string TableName { get; set; } = string.Empty;
    public long TableId { get; set; }
    public long ExternalId { get; set; }

    public long LogItemId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
}
