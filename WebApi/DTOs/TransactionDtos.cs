namespace Simple.Finance.WebApi.DTOs;

using System;

/// <summary>
/// Direction a search is sorted in
/// </summary>
public enum SearchOrder
{
    Asc,
    Desc,
}

/// <summary>
/// Transaction to create or update. Transfers between wallets are not created here,
/// they have their own endpoint
/// </summary>
public record TransactionRequest
{
    public long WalletId { get; set; }
    /// <summary>
    /// Category, must not be zero. It forces the sign of the values
    /// </summary>
    public long CategoryId { get; set; }
    /// <summary>
    /// Counterparty, 0 for none
    /// </summary>
    public long CounterpartyId { get; set; }

    public DateTime DueDate { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public Tables.Transac.PaymentStatus Status { get; set; }

    /// <summary>
    /// Must match the wallet's BaseCurrency, empty to inherit it
    /// </summary>
    public string PaymentCurrency { get; set; } = string.Empty;
    /// <summary>
    /// Value owed, never zero. Send it positive: the sign comes from the category
    /// </summary>
    public decimal DueValue { get; set; }
    /// <summary>
    /// Value actually paid, only meaningful when Status is Paid
    /// </summary>
    public decimal PaidValue { get; set; }
    public string? PaymentDetails { get; set; }
    /// <summary>
    /// The bank's own id for this movement
    /// </summary>
    public string? ExternalIdentifier { get; set; }

    /// <summary>
    /// A brand new row. The type is decided here: transfers have their own endpoint
    /// </summary>
    public Tables.Transac ToTable(long id) => ApplyTo(new Tables.Transac
    {
        Id = id,
        Type = Tables.Transac.TransactionType.Simple,
    });

    /// <summary>
    /// Writes the request over a row that already exists. Everything this API does not carry
    /// </summary>
    public Tables.Transac ApplyTo(Tables.Transac tx)
    {
        tx.WalletId = WalletId;
        tx.CategoryId = CategoryId;
        tx.CounterpartyId = CounterpartyId;
        tx.DueDate = DueDate;
        tx.PaymentDate = PaymentDate;
        tx.Description = Description;
        tx.Status = Status;
        tx.PaymentCurrency = PaymentCurrency;
        tx.DueValue = DueValue;
        tx.PaidValue = PaidValue;
        tx.PaymentDetails = PaymentDetails;
        tx.ExternalIdentifier = ExternalIdentifier;

        return tx;
    }

    /// <summary>
    /// Carries a transaction parsed by an importer
    /// </summary>
    public static TransactionRequest From(Tables.Transac tx) => new()
    {
        WalletId = tx.WalletId,
        CategoryId = tx.CategoryId,
        CounterpartyId = tx.CounterpartyId,
        DueDate = tx.DueDate,
        PaymentDate = tx.PaymentDate,
        Description = tx.Description,
        Status = tx.Status,
        PaymentCurrency = tx.PaymentCurrency,
        DueValue = tx.DueValue,
        PaidValue = tx.PaidValue,
        PaymentDetails = tx.PaymentDetails,
        ExternalIdentifier = tx.ExternalIdentifier,
    };
}

public record TransactionResponse
{
    public long Id { get; set; }
    public long WalletId { get; set; }
    public long CategoryId { get; set; }
    public long CounterpartyId { get; set; }

    public Tables.Transac.TransactionType Type { get; set; }
    /// <summary>
    /// The other leg of a wallet transfer, 0 otherwise
    /// </summary>
    public long TypeOtherId { get; set; }
    public Tables.Transac.PaymentStatus Status { get; set; }

    public DateTime DueDate { get; set; }
    public DateTime PaymentDate { get; set; }
    /// <summary>
    /// PaymentDate when paid, DueDate otherwise
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    public string Description { get; set; } = string.Empty;
    public string PaymentCurrency { get; set; } = string.Empty;
    public decimal DueValue { get; set; }
    public decimal PaidValue { get; set; }
    /// <summary>
    /// PaidValue when paid, DueValue otherwise
    /// </summary>
    public decimal EffectiveValue { get; set; }
    public string? PaymentDetails { get; set; }
    /// <summary>
    /// The bank's own id for this movement, null when there is none
    /// </summary>
    public string? ExternalIdentifier { get; set; }

    public DateTime Created { get; set; }
    public DateTime Changed { get; set; }

    public static TransactionResponse From(Tables.Transac tx) => new()
    {
        Id = tx.Id,
        WalletId = tx.WalletId,
        CategoryId = tx.CategoryId,
        CounterpartyId = tx.CounterpartyId,
        Type = tx.Type,
        TypeOtherId = tx.TypeOtherId,
        Status = tx.Status,
        DueDate = tx.DueDate,
        PaymentDate = tx.PaymentDate,
        EffectiveDate = tx.EffectiveDate,
        Description = tx.Description,
        PaymentCurrency = tx.PaymentCurrency,
        DueValue = tx.DueValue,
        PaidValue = tx.PaidValue,
        EffectiveValue = tx.EffectiveValue,
        PaymentDetails = tx.PaymentDetails,
        ExternalIdentifier = tx.ExternalIdentifier,
        Created = tx.Created,
        Changed = tx.Changed,
    };
}
