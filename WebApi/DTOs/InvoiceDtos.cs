namespace Simple.Finance.WebApi.DTOs;

using System;

/// <summary>
/// Invoice to create or update: a commercial document, not money.
/// It carries no wallet and no category, those belong to the transactions that settle it
/// </summary>
public record InvoiceRequest
{
    /// <summary>
    /// Counterparty of the document, 0 for none
    /// </summary>
    public long CounterpartyId { get; set; }

    /// <summary>
    /// Document number as printed, free text and optional: numbering is arbitrary per issuer,
    /// any series is part of it, and a Draft has no number yet
    /// </summary>
    public string Number { get; set; } = string.Empty;
    /// <summary>
    /// Identification of the fiscal document, when there is one. One free text column,
    /// because no jurisdiction owns this API
    /// </summary>
    public string? FiscalDocument { get; set; }
    /// <summary>
    /// Required: it is the only field that always names the document
    /// </summary>
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Date of the document, and the date the search window is applied over
    /// </summary>
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Currency code of the document. Nothing converts it
    /// </summary>
    public string Currency { get; set; } = string.Empty;
    /// <summary>
    /// Value of the document, never zero, and the only place its direction lives:
    /// negative is payable, positive is receivable. There is no boolean for that.
    /// The sign is frozen after creation, so an update that flips it answers 400.
    /// It is typed, never calculated from the items and never reconciled against them
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Non-negative magnitude already contained in <see cref="TotalValue"/>, informative only
    /// </summary>
    public decimal Taxes { get; set; }
    /// <summary>
    /// Non-negative magnitude already contained in <see cref="TotalValue"/>, informative only
    /// </summary>
    public decimal Fees { get; set; }
    /// <summary>
    /// Non-negative magnitude already contained in <see cref="TotalValue"/>, informative only
    /// </summary>
    public decimal Freight { get; set; }
    /// <summary>
    /// Non-negative magnitude already contained in <see cref="TotalValue"/>, informative only
    /// </summary>
    public decimal Discount { get; set; }
    public string? ValueDetails { get; set; }

    public string? PaymentTerms { get; set; }
    /// <summary>
    /// Reference given by the counterparty, such as an order or a contract
    /// </summary>
    public string? Reference { get; set; }
    /// <summary>
    /// Free slot for external applications: the library never reads it,
    /// it is stored and returned as sent
    /// </summary>
    public string? ExternalIdentifier { get; set; }

    /// <summary>
    /// Life cycle of the document. Cancellation is not a value here on purpose,
    /// it is its own flag so the status survives it
    /// </summary>
    public Tables.Invoice.InvoiceStatus Status { get; set; }
    /// <summary>
    /// Cancelled invoices are hidden, never deleted, and keep their Status.
    /// Use the dedicated endpoint to write it in bulk
    /// </summary>
    public bool IsCancelled { get; set; }

    public Tables.Invoice ToTable(long id) => new()
    {
        Id = id,
        CounterpartyId = CounterpartyId,
        Number = Number,
        FiscalDocument = FiscalDocument,
        Name = Name,
        Description = Description,
        IssueDate = IssueDate,
        DueDate = DueDate,
        Currency = Currency,
        TotalValue = TotalValue,
        Taxes = Taxes,
        Fees = Fees,
        Freight = Freight,
        Discount = Discount,
        ValueDetails = ValueDetails,
        PaymentTerms = PaymentTerms,
        Reference = Reference,
        ExternalIdentifier = ExternalIdentifier,
        Status = Status,
        IsCancelled = IsCancelled,
    };
}

public record InvoiceResponse
{
    public long Id { get; set; }
    public long CounterpartyId { get; set; }

    public string Number { get; set; } = string.Empty;
    public string? FiscalDocument { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }

    public string Currency { get; set; } = string.Empty;
    /// <summary>
    /// Negative is payable, positive is receivable: the sign is the direction of the document
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Magnitude already contained in <see cref="TotalValue"/>
    /// </summary>
    public decimal Taxes { get; set; }
    /// <summary>
    /// Magnitude already contained in <see cref="TotalValue"/>
    /// </summary>
    public decimal Fees { get; set; }
    /// <summary>
    /// Magnitude already contained in <see cref="TotalValue"/>
    /// </summary>
    public decimal Freight { get; set; }
    /// <summary>
    /// Magnitude already contained in <see cref="TotalValue"/>
    /// </summary>
    public decimal Discount { get; set; }
    public string? ValueDetails { get; set; }

    public string? PaymentTerms { get; set; }
    public string? Reference { get; set; }
    /// <summary>
    /// The external application's own id for this document, null when there is none
    /// </summary>
    public string? ExternalIdentifier { get; set; }

    public Tables.Invoice.InvoiceStatus Status { get; set; }
    /// <summary>
    /// Hidden document. The Status it had is preserved
    /// </summary>
    public bool IsCancelled { get; set; }

    public DateTime Created { get; set; }
    public DateTime Changed { get; set; }

    public static InvoiceResponse From(Tables.Invoice invoice) => new()
    {
        Id = invoice.Id,
        CounterpartyId = invoice.CounterpartyId,
        Number = invoice.Number,
        FiscalDocument = invoice.FiscalDocument,
        Name = invoice.Name,
        Description = invoice.Description,
        IssueDate = invoice.IssueDate,
        DueDate = invoice.DueDate,
        Currency = invoice.Currency,
        TotalValue = invoice.TotalValue,
        Taxes = invoice.Taxes,
        Fees = invoice.Fees,
        Freight = invoice.Freight,
        Discount = invoice.Discount,
        ValueDetails = invoice.ValueDetails,
        PaymentTerms = invoice.PaymentTerms,
        Reference = invoice.Reference,
        ExternalIdentifier = invoice.ExternalIdentifier,
        Status = invoice.Status,
        IsCancelled = invoice.IsCancelled,
        Created = invoice.Created,
        Changed = invoice.Changed,
    };
}

/// <summary>
/// Item of an invoice: one descriptive line of the document.
/// It has no wallet, no category and no transaction of its own, the payment is of the document
/// </summary>
public record InvoiceItemRequest
{
    /// <summary>
    /// Required: it is the only field that always names the line
    /// </summary>
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Zero is legal and means a removed or reversed line: the line stays on the document
    /// for the record, and its total collapses to zero
    /// </summary>
    public decimal Quantity { get; set; }
    /// <summary>
    /// Unit of the quantity, free text: kg, hour, piece, month
    /// </summary>
    public string? Unit { get; set; }
    /// <summary>
    /// Must not be zero. Send it positive: the sign comes from the document
    /// </summary>
    public decimal UnitValue { get; set; }
    /// <summary>
    /// Discount over the line, not over the unit, and never greater than
    /// <see cref="Quantity"/> * <see cref="UnitValue"/>. Taking the whole line is fine,
    /// the line total is then zero
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Product or service code given by the issuer, as printed
    /// </summary>
    public string? Code { get; set; }
    /// <summary>
    /// Free slot for external applications: the library never reads it,
    /// it is stored and returned as sent
    /// </summary>
    public string? ExternalIdentifier { get; set; }

    /// <summary>
    /// The line total is deliberately not carried here: the library calculates it as
    /// Quantity * UnitValue - Discount and signs it by the document, so a sent value
    /// would be silently overwritten. It comes back on the response
    /// </summary>
    public Tables.InvoiceItem ToTable(long id, long invoiceId) => new()
    {
        Id = id,
        InvoiceId = invoiceId,
        Name = Name,
        Description = Description,
        Quantity = Quantity,
        Unit = Unit,
        UnitValue = UnitValue,
        Discount = Discount,
        Code = Code,
        ExternalIdentifier = ExternalIdentifier,
    };
}

/// <summary>
/// Item of a bulk upsert, where each entry chooses between creating and updating
/// </summary>
public record InvoiceItemBulkRequest : InvoiceItemRequest
{
    /// <summary>
    /// 0 creates a new item, an existing id replaces that item
    /// </summary>
    public long Id { get; set; }
}

public record InvoiceItemResponse
{
    public long Id { get; set; }
    public long InvoiceId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal UnitValue { get; set; }
    /// <summary>
    /// Magnitude already contained in <see cref="TotalValue"/>
    /// </summary>
    public decimal Discount { get; set; }
    /// <summary>
    /// Total of the line as the library calculated it: Quantity * UnitValue - Discount,
    /// signed by the document. Zero when the discount takes the whole line,
    /// or when the quantity is zero
    /// </summary>
    public decimal TotalValue { get; set; }

    public string? Code { get; set; }
    /// <summary>
    /// The external application's own id for this line, null when there is none
    /// </summary>
    public string? ExternalIdentifier { get; set; }

    public static InvoiceItemResponse From(Tables.InvoiceItem item) => new()
    {
        Id = item.Id,
        InvoiceId = item.InvoiceId,
        Name = item.Name,
        Description = item.Description,
        Quantity = item.Quantity,
        Unit = item.Unit,
        UnitValue = item.UnitValue,
        Discount = item.Discount,
        TotalValue = item.TotalValue,
        Code = item.Code,
        ExternalIdentifier = item.ExternalIdentifier,
    };
}

/// <summary>
/// Mass toggle: the ids to write and the value to write on them.
/// Only the flag is written, every other field of those rows is left alone
/// </summary>
public record InvoiceToggleRequest
{
    public long[] Ids { get; set; } = [];
    /// <summary>
    /// The value IsCancelled takes. Cancelling hides the document and preserves its Status
    /// </summary>
    public bool State { get; set; }
}
