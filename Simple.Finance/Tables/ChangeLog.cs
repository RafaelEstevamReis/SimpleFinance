namespace Simple.Finance.Tables;

using Simple.DatabaseWrapper.Attributes;
using System;

public record ChangeLog
{
    public long Id { get; set; }
    [Index("ixChangeLog_Event")]
    public DateTime Event { get; set; }
    [Index("ixChangeLog_Table")]
    public string Table { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
}
