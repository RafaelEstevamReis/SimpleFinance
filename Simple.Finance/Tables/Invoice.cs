namespace Simple.Finance.Tables;

using Simple.DatabaseWrapper.Attributes;
using System;

public record Invoice
{
    /// <summary>
    /// Life cycle of the document
    /// </summary>
    public enum InvoiceStatus
    {
        Draft = 0,
        Sent = 1,
        Negotiation = 2,
        Active = 5,
        Rejected = 8,
        Finalized = 9,
    }

    [PrimaryKey]
    public long Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Changed { get; set; }

    [Index("ixInvoice_CounterpartyId")]
    public long CounterpartyId { get; set; }

    public string Number { get; set; } = string.Empty;
    public string? FiscalDocument { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Date of the document, the accrual anchor
    /// </summary>
    [Index("ixInvoice_IssueDate")]
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }

    public string Currency { get; set; } = string.Empty;
    /// <summary>
    /// Value of the document, negative is payable, positive is receivable.
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>Magnitude already contained in <see cref="TotalValue"/></summary>
    public decimal Taxes { get; set; }
    /// <summary>Magnitude already contained in <see cref="TotalValue"/></summary>
    public decimal Fees { get; set; }
    /// <summary>Magnitude already contained in <see cref="TotalValue"/></summary>
    public decimal Freight { get; set; }
    /// <summary>Magnitude already contained in <see cref="TotalValue"/></summary>
    public decimal Discount { get; set; }
    public string? ValueDetails { get; set; }

    public string? PaymentTerms { get; set; }
    public string? Reference { get; set; }
    public string? ExternalIdentifier { get; set; }

    [Index("ixInvoice_Status")]
    public InvoiceStatus Status { get; set; }
    public bool IsCancelled { get; set; }

    public override string ToString()
        => $"#{Id:0000} [{Status}{(IsCancelled ? "/Cancelled" : "")}] {IssueDate:d} {TotalValue:N2} {Name}";
}
