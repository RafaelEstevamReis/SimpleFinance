namespace Simple.Finance.WebApi.DTOs;

/// <summary>
/// Counterparty to create or update: employer, landlord, market, a friend
/// </summary>
public record PersonRequest
{
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Soft delete flag, nothing filters by it, the client decides what to show
    /// </summary>
    public bool IsDeleted { get; set; }

    public Tables.Person ToTable(long id) => new()
    {
        Id = id,
        Name = Name,
        IsDeleted = IsDeleted,
    };
}

public record PersonResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public static PersonResponse From(Tables.Person person) => new()
    {
        Id = person.Id,
        Name = person.Name,
        IsDeleted = person.IsDeleted,
    };
}
