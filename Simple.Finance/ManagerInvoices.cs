namespace Simple.Finance;

using Simple.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Invoices: the commercial side of the model.
/// It touches the financial side on a single bridge, <see cref="Tables.Transac.InvoiceId"/>,
/// and shares only <see cref="Tables.Person"/> with it.
/// </summary>
public partial class Manager
{
    #region Invoices

    public Tables.Invoice? GetInvoiceById(long invoiceId)
    {
        using var cnn = db.GetConnection();
        return cnn.Get<Tables.Invoice>(invoiceId);
    }

    /// <summary>
    /// Gets invoices issued on a date window. <see cref="Tables.Invoice.IssueDate"/> is the
    /// accrual anchor; the cash side is on the linked transactions
    /// </summary>
    public IEnumerable<Tables.Invoice> GetInvoices(DateTime start, DateTime end)
        => GetInvoicesBy(counterpartyId: null, status: null, isCancelled: null, start, end);

    /// <summary>
    /// Gets invoices issued on a date window, composing the optional cuts with AND
    /// </summary>
    public IEnumerable<Tables.Invoice> GetInvoicesBy(long? counterpartyId, Tables.Invoice.InvoiceStatus? status, bool? isCancelled, DateTime start, DateTime end)
    {
        string query = $"SELECT * FROM {nameof(Tables.Invoice)} WHERE {nameof(Tables.Invoice.IssueDate)} BETWEEN @start AND @end ";

        if (counterpartyId.HasValue) query += $" AND {nameof(Tables.Invoice.CounterpartyId)} = @counterpartyId ";
        if (status.HasValue) query += $" AND {nameof(Tables.Invoice.Status)} = @status ";
        if (isCancelled.HasValue) query += $" AND {nameof(Tables.Invoice.IsCancelled)} = @isCancelled ";

        query += $" ORDER BY {nameof(Tables.Invoice.IssueDate)}, Id";

        using var cnn = db.GetConnection();
        return cnn.Query<Tables.Invoice>(query, new
        {
            counterpartyId,
            // Unused when the clause above is absent, and never a nullable parameter
            status = status.GetValueOrDefault(),
            isCancelled = isCancelled.GetValueOrDefault(),
            start,
            end,
        });
    }

    public long CreateUpdateInvoice(Tables.Invoice invoice)
    {
        requireText(invoice.Name, nameof(Tables.Invoice), nameof(Tables.Invoice.Name));
        // The sign is the direction, so zero would be a document with no side
        if (invoice.TotalValue == 0) throw new InvalidOperationException($"'{nameof(Tables.Invoice)}.{nameof(Tables.Invoice.TotalValue)}' must not be zero");
        requireNotNegative(invoice.Taxes, nameof(Tables.Invoice), nameof(Tables.Invoice.Taxes));
        requireNotNegative(invoice.Fees, nameof(Tables.Invoice), nameof(Tables.Invoice.Fees));
        requireNotNegative(invoice.Freight, nameof(Tables.Invoice), nameof(Tables.Invoice.Freight));
        requireNotNegative(invoice.Discount, nameof(Tables.Invoice), nameof(Tables.Invoice.Discount));

        using var cnn = db.GetConnection();
        var originalValue = invoice.Id == 0 ? null : cnn.Get<Tables.Invoice>(invoice.Id);

        if (invoice.CounterpartyId != 0)
        {
            _ = cnn.Get<Tables.Person>(invoice.CounterpartyId) ?? throw new InvalidOperationException($"Invalid Counterparty Id: {invoice.CounterpartyId}");
        }

        if (originalValue != null && Math.Sign(originalValue.TotalValue) != Math.Sign(invoice.TotalValue))
        {
            throw new InvalidOperationException($"'{nameof(Tables.Invoice.TotalValue)}' sign cannot be changed");
        }

        invoice.Currency = invoice.Currency.ToUpper();

        invoice.Changed = DateTime.UtcNow;
        if (invoice.Id == 0) invoice.Created = DateTime.UtcNow;
        else if (originalValue != null) invoice.Created = originalValue.Created;

        // No ChangeLog and no notification: Created/Changed are the whole trace
        invoice.Id = cnn.Insert(invoice, OnConflict.Replace);
        return invoice.Id;
    }

    /// <summary>
    /// Cancels or uncancels many invoices at once, preserving the stage each one was on
    /// </summary>
    public void SetInvoiceCancelled(IEnumerable<long> ids, bool cancelledState)
        => setFlagByIds<Tables.Invoice>(ids, nameof(Tables.Invoice.IsCancelled), cancelledState);

    /// <summary>
    /// Gets the transactions settling an invoice. One invoice has many transactions,
    /// one transaction has at most one invoice
    /// </summary>
    public IEnumerable<Tables.Transac> GetInvoiceTransactions(long invoiceId)
    {
        using var cnn = db.GetConnection();
        return cnn.GetWhere<Tables.Transac>(nameof(Tables.Transac.InvoiceId), invoiceId);
    }

    #endregion

    #region Invoice Items

    public IEnumerable<Tables.InvoiceItem> GetInvoiceItems(long invoiceId)
    {
        using var cnn = db.GetConnection();
        return cnn.GetWhere<Tables.InvoiceItem>(nameof(Tables.InvoiceItem.InvoiceId), invoiceId);
    }

    public Tables.InvoiceItem? GetInvoiceItemById(long invoiceItemId)
    {
        using var cnn = db.GetConnection();
        return cnn.Get<Tables.InvoiceItem>(invoiceItemId);
    }

    public long CreateUpdateInvoiceItem(Tables.InvoiceItem item)
    {
        using var cnn = db.GetConnection();

        var invoice = cnn.Get<Tables.Invoice>(item.InvoiceId)
            ?? throw new InvalidOperationException($"Invalid Invoice Id: {item.InvoiceId}");

        applyInvoiceItemRules(item, invoice);

        item.Id = cnn.Insert(item, OnConflict.Replace);
        return item.Id;
    }

    /// <summary>
    /// Loads the referenced invoices once and then applies the rules per item.
    /// Like the transaction bulk it is not transactional: an invalid item throws and
    /// the previous ones stay
    /// </summary>
    public IEnumerable<long> CreateUpdateBulkInvoiceItem(IEnumerable<Tables.InvoiceItem> items)
    {
        var all = items.ToArray();

        using var cnn = db.GetConnection();
        var invoices = getByIds<Tables.Invoice>(cnn, all.Select(o => o.InvoiceId), o => o.Id);

        List<long> lst = [];
        foreach (var item in all)
        {
            if (!invoices.TryGetValue(item.InvoiceId, out var invoice))
            {
                throw new InvalidOperationException($"Invalid Invoice Id: {item.InvoiceId}");
            }

            applyInvoiceItemRules(item, invoice);

            item.Id = cnn.Insert(item, OnConflict.Replace);
            lst.Add(item.Id);
        }
        return lst;
    }

    public void DeleteInvoiceItem(long invoiceItemId)
    {
        using var cnn = db.GetConnection();
        cnn.Execute($"DELETE FROM {nameof(Tables.InvoiceItem)} WHERE Id = @invoiceItemId", new { invoiceItemId });
    }

    private static void applyInvoiceItemRules(Tables.InvoiceItem item, Tables.Invoice invoice)
    {
        requireText(item.Name, nameof(Tables.InvoiceItem), nameof(Tables.InvoiceItem.Name));
        if (item.UnitValue == 0) throw new InvalidOperationException($"'{nameof(Tables.InvoiceItem)}.{nameof(Tables.InvoiceItem.UnitValue)}' must not be zero");
        
        requireNotNegative(item.Quantity, nameof(Tables.InvoiceItem), nameof(Tables.InvoiceItem.Quantity));
        requireNotNegative(item.Discount, nameof(Tables.InvoiceItem), nameof(Tables.InvoiceItem.Discount));

        var sign = Math.Sign(invoice.TotalValue);
        item.UnitValue = Math.Abs(item.UnitValue) * sign;
        item.TotalValue = Math.Abs(item.TotalValue) * sign;
    }

    #endregion

    private static void requireNotNegative(decimal value, string table, string field)
    {
        if (value < 0) throw new InvalidOperationException($"'{table}.{field}' must not be negative");
    }
}
