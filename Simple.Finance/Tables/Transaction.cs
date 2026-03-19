namespace Simple.Finance.Tables;

using Simple.DatabaseWrapper.Attributes;
using System;
using System.Collections.Generic;

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
    public long CategoryId { get; set; }
    [Index("ixTransaction_WalletId")]
    [Index("ixTransaction_Balance", columnOrder: 1)]
    public long WalletId { get; set; }
    [Index("ixTransaction_CounterpartyId")]
    public long CounterpartyId { get; set; }
    public TransactionType Type { get; set; }
    public long TypeOtherId { get; set; }

    [Index("ixTransaction_Balance", columnOrder: 0)] // First column on index replaces a solo index on the column
    public PaymentStatus Status { get; set; }
    public string PaymentCurrency { get; set; } = string.Empty;
    public decimal DueValue { get; set; }
    [Index("ixTransaction_Balance", columnOrder: 2)]
    public decimal PaidValue { get; set; }

    public string ReferenceCurrency { get; set; } = string.Empty;
    public decimal RC_DueValue { get; set; }
    public decimal RC_PaidValue { get; set; }

    public string? ExternalIdentifier { get; set; }

    public string? PaymentDetails { get; set; }

    public DateTime EfectiveDate => Status == PaymentStatus.Paid ? PaymentDate : DueDate.Date;
    public decimal EfectiveValue => Status == PaymentStatus.Paid ? PaidValue : DueValue;

    /// <summary>
    /// Get Transaction category name using provided cache
    /// </summary>
    public string GetCategoryName(Dictionary<long, string> categories)
    {
        if(categories.ContainsKey(CategoryId)) return categories[CategoryId];
        return "[-]";
    }
    /// <summary>
    /// Get Transaction category name using provided cache
    /// </summary>
    public string GetCategoryName(Dictionary<long, Category> categories)
    {
        if (categories.ContainsKey(CategoryId)) return categories[CategoryId].Name;
        return "[-]";
    }
    /// <summary>
    /// Get Transaction wallet name using provided cache
    /// </summary>
    public string GetWalletName(Dictionary<long, string> wallets)
    {
        if (wallets.ContainsKey(WalletId)) return wallets[WalletId];
        return "[-]";
    }
    /// <summary>
    /// Get Transaction wallet name using provided cache
    /// </summary>
    public string GetWalletName(Dictionary<long, Wallet> wallets)
    {
        if (wallets.ContainsKey(WalletId)) return wallets[WalletId].Name;
        return "[-]";
    }
    /// <summary>
    /// Get transaction PaymentCurrency code, if empty, wallet's BaseCurrency is returned instead
    /// </summary>
    public string GetTransacationCurrencyCode(Dictionary<long, Wallet> wallets)
    {
        if (!string.IsNullOrEmpty(PaymentCurrency)) return PaymentCurrency;
        if (wallets.ContainsKey(WalletId)) return wallets[WalletId].BaseCurrency;

        return string.Empty;
    }

    public override string ToString() 
        => $"#{Id:0000} [{Type}/{Status, -6}] {EfectiveDate:d} {EfectiveValue:N2} {Description}";
}
