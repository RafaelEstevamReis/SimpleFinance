namespace Simple.Finance.WebApi.DTOs;

/// <summary>
/// Wallet to create or update: an account, a card, cash, a broker
/// </summary>
public record WalletRequest
{
    /// <summary>
    /// Free slot for external applications: the library never reads or validates it,
    /// it is stored and returned as sent. 0 means unused
    /// </summary>
    public int ExternallType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Currency of everything inside this wallet. Transactions must match it
    /// </summary>
    public string BaseCurrency { get; set; } = string.Empty;
    /// <summary>
    /// Soft delete flag, nothing filters by it, the client decides what to show
    /// </summary>
    public bool IsDeleted { get; set; }

    public Tables.Wallet ToTable(long id) => new()
    {
        Id = id,
        ExternallType = ExternallType,
        Name = Name,
        Description = Description,
        BaseCurrency = BaseCurrency,
        IsDeleted = IsDeleted,
    };
}

public record WalletResponse
{
    public long Id { get; set; }
    /// <summary>
    /// Free slot for external applications, stored and returned as sent. 0 means unused
    /// </summary>
    public int ExternallType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public static WalletResponse From(Tables.Wallet wallet) => new()
    {
        Id = wallet.Id,
        ExternallType = wallet.ExternallType,
        Name = wallet.Name,
        Description = wallet.Description,
        BaseCurrency = wallet.BaseCurrency,
        IsDeleted = wallet.IsDeleted,
    };
}

/// <summary>
/// Balance of a wallet, always in the wallet's own BaseCurrency.
/// Balances of different currencies are never summed
/// </summary>
public record WalletBalanceResponse
{
    public long WalletId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
