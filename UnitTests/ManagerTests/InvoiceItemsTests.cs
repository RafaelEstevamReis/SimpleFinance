namespace UnitTests.ManagerTests;

using Simple.Finance.Tables;
using System;
using System.Linq;
using Xunit;

public class InvoiceItemsTests : ManagerTestBase
{
    readonly long invoiceId;

    public InvoiceItemsTests()
    {
        invoiceId = newInvoice();
    }

    long newInvoice(decimal total = 1000m)
        => mgr.CreateUpdateInvoice(new Invoice
        {
            Id = 0,
            Name = "Invoice",
            IssueDate = past,
            DueDate = past.AddDays(30),
            Currency = "BRL",
            TotalValue = total,
        });

    static InvoiceItem item(long invoiceId, decimal unitValue = 10m, decimal total = 20m)
        => new()
        {
            Id = 0,
            InvoiceId = invoiceId,
            Name = "line",
            Quantity = 2m,
            Unit = "pc",
            UnitValue = unitValue,
            TotalValue = total,
        };

    [Fact]
    public void Create_StoresTheLine()
    {
        var id = mgr.CreateUpdateInvoiceItem(new InvoiceItem
        {
            Id = 0,
            InvoiceId = invoiceId,
            Name = "Coffee",
            Description = "ground, 500g",
            Quantity = 1.5m,
            Unit = "kg",
            UnitValue = 40m,
            Discount = 5m,
            TotalValue = 55m,
            Code = "SKU-7",
            ExternalIdentifier = "ext-9",
        });

        var stored = mgr.GetInvoiceItemById(id);

        Assert.NotNull(stored);
        Assert.Equal(invoiceId, stored.InvoiceId);
        Assert.Equal("Coffee", stored.Name);
        Assert.Equal("ground, 500g", stored.Description);
        Assert.Equal(1.5m, stored.Quantity);
        Assert.Equal("kg", stored.Unit);
        Assert.Equal(40m, stored.UnitValue);
        Assert.Equal(5m, stored.Discount);
        Assert.Equal(55m, stored.TotalValue);
        Assert.Equal("SKU-7", stored.Code);
        Assert.Equal("ext-9", stored.ExternalIdentifier);
    }

    [Fact]
    public void Create_WithoutName_Throws()
    {
        var line = item(invoiceId);
        line.Name = "";

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoiceItem(line));
        Assert.Contains($"{nameof(InvoiceItem)}.{nameof(InvoiceItem.Name)}", ex.Message);
    }

    [Fact]
    public void Create_WithZeroUnitValue_Throws()
    {
        var line = item(invoiceId, unitValue: 0m);

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoiceItem(line));
        Assert.Contains(nameof(InvoiceItem.UnitValue), ex.Message);
    }

    [Fact]
    public void Create_WithZeroTotal_IsAccepted()
    {
        // A line fully taken by its own discount is a fact, not an invalid state.
        // The sign here is inherited, so zero carries no ambiguity
        var line = item(invoiceId, unitValue: 10m, total: 0m);
        line.Discount = 20m;

        var id = mgr.CreateUpdateInvoiceItem(line);

        Assert.Equal(0m, mgr.GetInvoiceItemById(id)!.TotalValue);
    }

    [Fact]
    public void Create_WithNegativeQuantity_Throws()
    {
        var line = item(invoiceId);
        line.Quantity = -1m;

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoiceItem(line));
        Assert.Contains(nameof(InvoiceItem.Quantity), ex.Message);
    }

    [Fact]
    public void Create_WithNegativeDiscount_Throws()
    {
        var line = item(invoiceId);
        line.Discount = -1m;

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoiceItem(line));
        Assert.Contains($"{nameof(InvoiceItem)}.{nameof(InvoiceItem.Discount)}", ex.Message);
    }

    [Theory]
    [InlineData(1000, 1)]
    [InlineData(-1000, -1)]
    public void Create_InheritsTheSignOfTheDocument(decimal documentTotal, int expectedSign)
    {
        var ownInvoice = newInvoice(documentTotal);

        // Both signs offered, so the result is the document's and not the caller's
        var line = item(ownInvoice, unitValue: -10m, total: 20m);
        var id = mgr.CreateUpdateInvoiceItem(line);

        var stored = mgr.GetInvoiceItemById(id)!;
        Assert.Equal(10m * expectedSign, stored.UnitValue);
        Assert.Equal(20m * expectedSign, stored.TotalValue);
    }

    [Fact]
    public void Create_WithInvalidInvoice_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateInvoiceItem(item(999)));
        Assert.Contains("Invoice", ex.Message);
    }

    [Fact]
    public void Create_DoesNotRecalculateTheTotal()
    {
        // Quantity * UnitValue - Discount is 15, and the typed total wins:
        // keeping them consistent belongs to whoever uses the library
        var line = item(invoiceId, unitValue: 10m, total: 999m);
        line.Quantity = 2m;
        line.Discount = 5m;

        var id = mgr.CreateUpdateInvoiceItem(line);

        Assert.Equal(999m, mgr.GetInvoiceItemById(id)!.TotalValue);
    }

    [Fact]
    public void Update_ReplacesTheSameLine()
    {
        var id = mgr.CreateUpdateInvoiceItem(item(invoiceId));

        var stored = mgr.GetInvoiceItemById(id)!;
        stored.Name = "renamed";
        mgr.CreateUpdateInvoiceItem(stored);

        var single = Assert.Single(mgr.GetInvoiceItems(invoiceId));
        Assert.Equal(id, single.Id);
        Assert.Equal("renamed", single.Name);
    }

    [Fact]
    public void Bulk_CreatesEveryLine()
    {
        var ids = mgr.CreateUpdateBulkInvoiceItem([item(invoiceId), item(invoiceId), item(invoiceId)]).ToArray();

        Assert.Equal(3, ids.Length);
        Assert.Equal(3, mgr.GetInvoiceItems(invoiceId).Count());
        Assert.Equal(3, ids.Distinct().Count());
    }

    [Fact]
    public void Bulk_AppliesTheSignOfEachParentDocument()
    {
        var payable = newInvoice(-500m);

        var ids = mgr.CreateUpdateBulkInvoiceItem([item(invoiceId), item(payable)]).ToArray();

        Assert.Equal(20m, mgr.GetInvoiceItemById(ids[0])!.TotalValue);
        Assert.Equal(-20m, mgr.GetInvoiceItemById(ids[1])!.TotalValue);
    }

    [Fact]
    public void Bulk_WithInvalidInvoice_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateBulkInvoiceItem([item(invoiceId), item(999)]).ToArray());
    }

    [Fact]
    public void Items_WriteNoChangeLog()
    {
        var id = mgr.CreateUpdateInvoiceItem(item(invoiceId));

        Assert.Empty(mgr.GetLogs<InvoiceItem>(id));
    }

    [Fact]
    public void Delete_RemovesOnlyThatLine()
    {
        var first = mgr.CreateUpdateInvoiceItem(item(invoiceId));
        var second = mgr.CreateUpdateInvoiceItem(item(invoiceId));

        mgr.DeleteInvoiceItem(first);

        var single = Assert.Single(mgr.GetInvoiceItems(invoiceId));
        Assert.Equal(second, single.Id);
    }

    [Fact]
    public void GetInvoiceItems_ReturnsOnlyThatDocument()
    {
        var other = newInvoice();
        mgr.CreateUpdateInvoiceItem(item(invoiceId));
        mgr.CreateUpdateInvoiceItem(item(other));
        mgr.CreateUpdateInvoiceItem(item(other));

        Assert.Single(mgr.GetInvoiceItems(invoiceId));
        Assert.Equal(2, mgr.GetInvoiceItems(other).Count());
    }
}
