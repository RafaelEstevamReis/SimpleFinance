namespace Simple.Finance.Tables;

using Simple.DatabaseWrapper.Attributes;
using System;

public record Transac
{
    public enum TransactionType
    {
        Simple = 0,
        WalletTransfer = 1,
        Special = 9,
    }
    public enum PaymentStatus
    {
        Unpaid = 0,
        Paid = 1,
        Reversed = 9,
    }

    [PrimaryKey]
    public long Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Changed { get; set; }
    [Index("ixTransaction_DueDate")]
    public DateTime DueDate { get; set; }
    [Index("ixTransaction_PaymentDate")]
    public DateTime PaymentDate { get; set; }
    public string Description { get; set; } = string.Empty;
    [Index("ixTransaction_CategoryId")]
    public int CategoryId { get; set; }
    [Index("ixTransaction_WalletId")]
    public int WalletId { get; set; }
    [Index("ixTransaction_CounterpartyId")]
    public int CounterpartyId { get; set; }
    public TransactionType Type { get; set; }
    public long TypeOtherId { get; set; }

    public PaymentStatus Status { get; set; }
    public string PaymentCurrency { get; set; } = string.Empty;
    public decimal DueValue { get; set; }
    public decimal PaidValue { get; set; }

    public string ReferenceCurrency { get; set; } = string.Empty;
    public decimal RC_DueValue { get; set; }
    public decimal RC_PaidValue { get; set; }

}
