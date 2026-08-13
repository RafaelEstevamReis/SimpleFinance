namespace Simple.Finance.Tables;

using Simple.DatabaseWrapper.Attributes;

public record Category
{
    [PrimaryKey]
    public long Id { get; set; }
    public bool IsExpense { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MonthlyBudget {  get; set; }
    public bool IsDeleted { get; set; }
}
