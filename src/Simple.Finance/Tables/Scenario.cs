namespace Simple.Finance.Tables;

using Simple.DatabaseWrapper.Attributes;

public record Scenario
{
    [PrimaryKey]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
