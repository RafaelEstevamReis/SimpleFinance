namespace Simple.Finance.Tables;

using Simple.DatabaseWrapper.Attributes;

public record InvoiceItem
{
    [PrimaryKey]
    public long Id { get; set; }
    [Index("ixInvoiceItem_InvoiceId")]
    public long InvoiceId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Quantity { get; set; }
    /// <summary>Unit of the quantity, free text: kg, hour, piece, month</summary>
    public string? Unit { get; set; }
    public decimal UnitValue { get; set; }
    /// <summary>
    /// Discount over the line, not over the unit. Magnitude already contained in <see cref="TotalValue"/>
    /// </summary>
    public decimal Discount { get; set; }
    /// <summary>
    /// Total of the line, calculated as <see cref="Quantity"/> * <see cref="UnitValue"/> - <see cref="Discount"/>
    /// and signed by the document. Zero when the discount takes the whole line
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>Product or service code given by the issuer, as printed</summary>
    public string? Code { get; set; }
    public string? ExternalIdentifier { get; set; }
}
