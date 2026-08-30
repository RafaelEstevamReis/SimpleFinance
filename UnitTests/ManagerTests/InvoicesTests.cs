namespace UnitTests.ManagerTests;

using Simple.Finance;
using Simple.Finance.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class InvoicesTests : ManagerTestBase
{
    static Invoice invoice(decimal total = 1000m, long counterpartyId = 0, DateTime? issueDate = null)
        => new()
        {
            Id = 0,
            CounterpartyId = counterpartyId,
            Name = "Invoice",
            Number = "NF-1",
            IssueDate = issueDate ?? past,
            DueDate = (issueDate ?? past).AddDays(30),
            Currency = "brl",
            TotalValue = total,
        };

    [Fact]
    public void Create_StoresTheDocument()
    {
        var personId = newPerson("Client");

        var id = mgr.CreateUpdateInvoice(new Invoice
        {
            Id = 0,
            CounterpartyId = personId,
            Name = "Consulting",
            Number = "NF-2024/17",
            FiscalDocument = "fiscal-key-42",
            Description = "March",
            IssueDate = past,
            DueDate = past.AddDays(15),
            Currency = "USD",
            TotalValue = 2500m,
            Taxes = 100m,
            Fees = 20m,
            Freight = 30m,
            Discount = 50m,
            ValueDetails = "withheld at source",
            PaymentTerms = "net 15",
            Reference = "PO-9",
            ExternalIdentifier = "ext-1",
            Status = Invoice.InvoiceStatus.Active,
        });

        var stored = mgr.GetInvoiceById(id);

        Assert.NotNull(stored);
        Assert.Equal(personId, stored.CounterpartyId);
        Assert.Equal("Consulting", stored.Name);
        Assert.Equal("NF-2024/17", stored.Number);
        Assert.Equal("fiscal-key-42", stored.FiscalDocument);
        Assert.Equal("USD", stored.Currency);
        Assert.Equal(2500m, stored.TotalValue);
        Assert.Equal(100m, stored.Taxes);
        Assert.Equal(20m, stored.Fees);
        Assert.Equal(30m, stored.Freight);
        Assert.Equal(50m, stored.Discount);
        Assert.Equal("net 15", stored.PaymentTerms);
        Assert.Equal("PO-9", stored.Reference);
        Assert.Equal(Invoice.InvoiceStatus.Active, stored.Status);
        Assert.False(stored.IsCancelled);
    }

    [Fact]
    public void Create_UpperCasesTheCurrency()
    {
        var id = mgr.CreateUpdateInvoice(invoice());

        Assert.Equal("BRL", mgr.GetInvoiceById(id)!.Currency);
    }

    [Fact]
    public void Create_WithoutName_Throws()
    {
        var inv = invoice();
        inv.Name = "";

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoice(inv));
        Assert.Contains($"{nameof(Invoice)}.{nameof(Invoice.Name)}", ex.Message);
    }

    [Fact]
    public void Create_WithoutNumber_IsAccepted()
    {
        // A Draft has no number yet: numbering is assigned on issue
        var inv = invoice();
        inv.Number = "";
        inv.Status = Invoice.InvoiceStatus.Draft;

        var id = mgr.CreateUpdateInvoice(inv);

        Assert.Equal("", mgr.GetInvoiceById(id)!.Number);
    }

    [Fact]
    public void Create_WithZeroTotal_Throws()
    {
        var inv = invoice(total: 0m);

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoice(inv));
        Assert.Contains(nameof(Invoice.TotalValue), ex.Message);
    }

    [Theory]
    [InlineData(nameof(Invoice.Taxes))]
    [InlineData(nameof(Invoice.Fees))]
    [InlineData(nameof(Invoice.Freight))]
    [InlineData(nameof(Invoice.Discount))]
    public void Create_WithNegativeMagnitude_Throws(string field)
    {
        var inv = invoice();
        switch (field)
        {
            case nameof(Invoice.Taxes): inv.Taxes = -1m; break;
            case nameof(Invoice.Fees): inv.Fees = -1m; break;
            case nameof(Invoice.Freight): inv.Freight = -1m; break;
            case nameof(Invoice.Discount): inv.Discount = -1m; break;
        }

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoice(inv));
        Assert.Contains(field, ex.Message);
    }

    [Fact]
    public void Create_WithInvalidCounterparty_Throws()
    {
        var inv = invoice(counterpartyId: 999);

        Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoice(inv));
    }

    [Fact]
    public void Create_WithoutCounterparty_IsAccepted()
    {
        // Somebody just recording what they bought does not create a Person for every store
        var id = mgr.CreateUpdateInvoice(invoice(counterpartyId: 0));

        Assert.Equal(0, mgr.GetInvoiceById(id)!.CounterpartyId);
    }

    [Fact]
    public void Update_PreservesCreatedAndMovesChanged()
    {
        var id = mgr.CreateUpdateInvoice(invoice());
        var created = mgr.GetInvoiceById(id)!.Created;

        var again = mgr.GetInvoiceById(id)!;
        again.Name = "Renamed";
        mgr.CreateUpdateInvoice(again);

        var stored = mgr.GetInvoiceById(id)!;
        Assert.Equal(created, stored.Created);
        Assert.True(stored.Changed >= created);
        Assert.Equal("Renamed", stored.Name);
    }

    [Fact]
    public void Update_ChangingTheSign_Throws()
    {
        var id = mgr.CreateUpdateInvoice(invoice(total: 1000m));

        var stored = mgr.GetInvoiceById(id)!;
        stored.TotalValue = -1000m;

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoice(stored));
        Assert.Contains(nameof(Invoice.TotalValue), ex.Message);
    }

    [Fact]
    public void Update_KeepingTheSign_ChangesTheValue()
    {
        var id = mgr.CreateUpdateInvoice(invoice(total: 1000m));

        var stored = mgr.GetInvoiceById(id)!;
        stored.TotalValue = 1500m;
        mgr.CreateUpdateInvoice(stored);

        Assert.Equal(1500m, mgr.GetInvoiceById(id)!.TotalValue);
    }

    [Fact]
    public void Invoices_WriteNoChangeLog()
    {
        var id = mgr.CreateUpdateInvoice(invoice());
        var stored = mgr.GetInvoiceById(id)!;
        stored.Name = "Renamed";
        mgr.CreateUpdateInvoice(stored);

        Assert.Empty(mgr.GetLogs<Invoice>(id));
    }

    [Fact]
    public void Invoices_FireNoNotification()
    {
        List<ManagerNotificationEventArgs> events = [];
        mgr.EventNotifier += (s, e) => events.Add(e);

        mgr.CreateUpdateInvoice(invoice());

        Assert.Empty(events);
    }

    [Fact]
    public void GetInvoices_ReturnsOnlyTheIssueDateWindow()
    {
        mgr.CreateUpdateInvoice(invoice(issueDate: past));
        mgr.CreateUpdateInvoice(invoice(issueDate: past.AddYears(2)));

        var found = mgr.GetInvoices(past.AddDays(-1), past.AddDays(1)).ToArray();

        Assert.Single(found);
        Assert.Equal(past, found[0].IssueDate);
    }

    [Fact]
    public void GetInvoicesBy_ComposesTheCuts()
    {
        var clientA = newPerson("A");
        var clientB = newPerson("B");

        var wanted = invoice(counterpartyId: clientA);
        wanted.Status = Invoice.InvoiceStatus.Active;
        var wantedId = mgr.CreateUpdateInvoice(wanted);

        var otherPerson = invoice(counterpartyId: clientB);
        otherPerson.Status = Invoice.InvoiceStatus.Active;
        mgr.CreateUpdateInvoice(otherPerson);

        var otherStatus = invoice(counterpartyId: clientA);
        otherStatus.Status = Invoice.InvoiceStatus.Draft;
        mgr.CreateUpdateInvoice(otherStatus);

        var found = mgr.GetInvoicesBy(clientA, Invoice.InvoiceStatus.Active, isCancelled: false, past.AddDays(-1), past.AddDays(1)).ToArray();

        Assert.Single(found);
        Assert.Equal(wantedId, found[0].Id);
    }

    [Fact]
    public void SetInvoiceCancelled_PreservesTheStatus()
    {
        var inv = invoice();
        inv.Status = Invoice.InvoiceStatus.Negotiation;
        var id = mgr.CreateUpdateInvoice(inv);

        mgr.SetInvoiceCancelled([id], true);

        var stored = mgr.GetInvoiceById(id)!;
        Assert.True(stored.IsCancelled);
        Assert.Equal(Invoice.InvoiceStatus.Negotiation, stored.Status);
    }

    [Fact]
    public void Delete_RemovesTheInvoiceAndItsItems()
    {
        var id = mgr.CreateUpdateInvoice(invoice());
        mgr.CreateUpdateInvoiceItem(new InvoiceItem
        {
            Id = 0,
            InvoiceId = id,
            Name = "line",
            Quantity = 1m,
            UnitValue = 10m,
            TotalValue = 10m,
        });

        mgr.DeleteInvoice(id);

        Assert.Null(mgr.GetInvoiceById(id));
        Assert.Empty(mgr.GetInvoiceItems(id));
    }

    [Fact]
    public void Delete_WithLinkedTransaction_Throws()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);
        var invoiceId = mgr.CreateUpdateInvoice(invoice(total: -500m));

        var t = tx(walletId, categoryId, 500m, past);
        t.InvoiceId = invoiceId;
        mgr.CreateUpdateTransaction(t);

        Assert.Throws<InvalidOperationException>(() => mgr.DeleteInvoice(invoiceId));
        Assert.NotNull(mgr.GetInvoiceById(invoiceId));
    }

    [Fact]
    public void GetInvoiceTransactions_ReturnsOnlyTheLinkedOnes()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);
        var invoiceId = mgr.CreateUpdateInvoice(invoice(total: -500m));

        var linked = tx(walletId, categoryId, 200m, past);
        linked.InvoiceId = invoiceId;
        var linkedId = mgr.CreateUpdateTransaction(linked);

        var second = tx(walletId, categoryId, 300m, past.AddDays(30));
        second.InvoiceId = invoiceId;
        var secondId = mgr.CreateUpdateTransaction(second);

        newTx(walletId, categoryId, 99m, past);

        var found = mgr.GetInvoiceTransactions(invoiceId).Select(o => o.Id).ToArray();

        Assert.Equal(2, found.Length);
        Assert.Contains(linkedId, found);
        Assert.Contains(secondId, found);
    }

    [Fact]
    public void Transaction_WithInvalidInvoiceId_Throws()
    {
        var walletId = newWallet();
        var categoryId = newCategory(isExpense: true);

        var t = tx(walletId, categoryId, 100m, past);
        t.InvoiceId = 999;

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateTransaction(t));
        Assert.Contains("Invoice", ex.Message);
    }
}
