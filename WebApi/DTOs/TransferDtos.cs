namespace Simple.Finance.WebApi.DTOs;

using System;

/// <summary>
/// Money moving between two wallets. It writes two linked transactions,
/// negative on the source and positive on the destination
/// </summary>
public record CreateTransferRequest
{
    public long SourceWalletId { get; set; }
    /// <summary>
    /// Category of the outgoing leg, must be an expense. 0 for none
    /// </summary>
    public long SourceCategoryId { get; set; }
    public long DestinationWalletId { get; set; }
    /// <summary>
    /// Category of the incoming leg, must not be an expense. 0 for none
    /// </summary>
    public long DestinationCategoryId { get; set; }

    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Amount to move, always positive
    /// </summary>
    public decimal Value { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime PaymentDate { get; set; }
    public bool Paid { get; set; }
    public string? PaymentDetails { get; set; }
}

/// <summary>
/// New state of both legs of a transfer
/// </summary>
public record UpdateTransferRequest
{
    public decimal DueValue { get; set; }
    public decimal PaidValue { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PaymentDetails { get; set; }
    public Tables.Transac.PaymentStatus Status { get; set; }
}

/// <summary>
/// Both legs of a transfer
/// </summary>
public record TransferResponse
{
    /// <summary>
    /// Leg that left the source wallet, always negative
    /// </summary>
    public TransactionResponse Source { get; set; } = new();
    /// <summary>
    /// Leg that entered the destination wallet, always positive
    /// </summary>
    public TransactionResponse Destination { get; set; } = new();
}
