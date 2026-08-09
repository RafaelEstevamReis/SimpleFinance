namespace Simple.Finance.WebApi.DTOs;

/// <summary>
/// Category to create or update. It is what decides the sign of a transaction
/// </summary>
public record CategoryRequest
{
    /// <summary>
    /// True for expenses (negative values), false for income (positive).
    /// Cannot be changed after the category is created
    /// </summary>
    public bool IsExpense { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Soft delete flag, nothing filters by it, the client decides what to show
    /// </summary>
    public bool IsDeleted { get; set; }

    public Tables.Category ToTable(long id) => new()
    {
        Id = id,
        IsExpense = IsExpense,
        Name = Name,
        Description = Description,
        IsDeleted = IsDeleted,
    };
}

public record CategoryResponse
{
    public long Id { get; set; }
    public bool IsExpense { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public static CategoryResponse From(Tables.Category category) => new()
    {
        Id = category.Id,
        IsExpense = category.IsExpense,
        Name = category.Name,
        Description = category.Description,
        IsDeleted = category.IsDeleted,
    };
}
