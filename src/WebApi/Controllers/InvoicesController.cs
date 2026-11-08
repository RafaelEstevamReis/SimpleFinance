namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.DTOs;
using System;
using System.Linq;

/// <summary>
/// Invoices: commercial documents, not money. The money is on the transactions that settle them,
/// and that is where the wallet and the category live.
/// Nothing here is logged on the change log and nothing here raises a notification
/// </summary>
public class InvoicesController(ManagerCache managers) : AccountControllerBase(managers)
{
    /// <summary>
    /// Invoices issued on a date window, the window being applied over IssueDate.
    /// The optional cuts compose with AND: omitting one keeps every value of it
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(InvoiceResponse[]), StatusCodes.Status200OK)]
    public ActionResult<InvoiceResponse[]> Search([FromQuery] DateTime start,
                                                  [FromQuery] DateTime end,
                                                  [FromQuery] long? counterpartyId,
                                                  [FromQuery] Tables.Invoice.InvoiceStatus? status,
                                                  [FromQuery] bool? isCancelled)
        => Manager.GetInvoicesBy(counterpartyId, status, isCancelled, start, end)
            .Select(InvoiceResponse.From).ToArray();

    /// <summary>
    /// A single invoice, cancelled or not
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<InvoiceResponse> Get(long id)
    {
        var invoice = Manager.GetInvoiceById(id);
        if (invoice is null) return NotFound();

        return InvoiceResponse.From(invoice);
    }

    /// <summary>
    /// Creates an invoice. The sign of TotalValue is the direction of the document,
    /// negative payable and positive receivable, and it is fixed from here on
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<InvoiceResponse> Create([FromBody] InvoiceRequest request)
    {
        var invoice = request.ToTable(0);
        Manager.CreateUpdateInvoice(invoice);

        return CreatedAtAction(nameof(Get), new { id = invoice.Id }, InvoiceResponse.From(invoice));
    }

    /// <summary>
    /// Updates an invoice. Every field is rewritten, so the sign of TotalValue must be the one
    /// it was created with: flipping a payable into a receivable answers 400
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<InvoiceResponse> Update(long id, [FromBody] InvoiceRequest request)
    {
        if (Manager.GetInvoiceById(id) is null) return NotFound();

        var invoice = request.ToTable(id);
        Manager.CreateUpdateInvoice(invoice);

        return InvoiceResponse.From(invoice);
    }

    /// <summary>
    /// Cancels or uncancels many invoices at once. Invoices are documents that happened,
    /// so they are hidden and never deleted: only IsCancelled is written and the Status
    /// each one was on is preserved.
    /// The literal segment cannot be reached by the id routes, they only take numbers
    /// </summary>
    [HttpPut("cancelled")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SetCancelled([FromBody] InvoiceToggleRequest request)
    {
        Manager.SetInvoiceCancelled(request.Ids, request.State);
        return NoContent();
    }

    /// <summary>
    /// Transactions settling one invoice. One invoice has many transactions,
    /// one transaction has at most one invoice
    /// </summary>
    [HttpGet("{id:long}/transactions")]
    [ProducesResponseType(typeof(TransactionResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TransactionResponse[]> GetTransactions(long id)
    {
        if (Manager.GetInvoiceById(id) is null) return NotFound();

        return Manager.GetInvoiceTransactions(id).Select(TransactionResponse.From).ToArray();
    }

    /// <summary>
    /// Items of one invoice. They are descriptive: their sum is informative and
    /// the document's TotalValue is never reconciled against them
    /// </summary>
    [HttpGet("{id:long}/items")]
    [ProducesResponseType(typeof(InvoiceItemResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<InvoiceItemResponse[]> GetItems(long id)
    {
        if (Manager.GetInvoiceById(id) is null) return NotFound();

        return Manager.GetInvoiceItems(id).Select(InvoiceItemResponse.From).ToArray();
    }

    /// <summary>
    /// A single item of an invoice
    /// </summary>
    [HttpGet("{id:long}/items/{itemId:long}")]
    [ProducesResponseType(typeof(InvoiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<InvoiceItemResponse> GetItem(long id, long itemId)
    {
        var item = findItem(id, itemId);
        if (item is null) return NotFound();

        return InvoiceItemResponse.From(item);
    }

    /// <summary>
    /// Adds an item to an invoice. The line total is not sent, it is calculated as
    /// Quantity * UnitValue - Discount and signed by the document, and comes back on the response
    /// </summary>
    [HttpPost("{id:long}/items")]
    [ProducesResponseType(typeof(InvoiceItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<InvoiceItemResponse> CreateItem(long id, [FromBody] InvoiceItemRequest request)
    {
        if (Manager.GetInvoiceById(id) is null) return NotFound();

        var item = request.ToTable(0, id);
        Manager.CreateUpdateInvoiceItem(item);

        return CreatedAtAction(nameof(GetItem), new { id, itemId = item.Id }, InvoiceItemResponse.From(item));
    }

    /// <summary>
    /// Adds or replaces many items at once, on a single connection.
    /// Entries are applied in order, so an invalid one answers 400 and leaves the previous ones stored
    /// </summary>
    [HttpPost("{id:long}/items/bulk")]
    [ProducesResponseType(typeof(InvoiceItemResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<InvoiceItemResponse[]> CreateItemsBulk(long id, [FromBody] InvoiceItemBulkRequest[] request)
    {
        if (Manager.GetInvoiceById(id) is null) return NotFound();

        var items = request.Select(o => o.ToTable(o.Id, id)).ToArray();
        Manager.CreateUpdateBulkInvoiceItem(items);

        return items.Select(InvoiceItemResponse.From).ToArray();
    }

    /// <summary>
    /// Replaces an item of an invoice. A quantity of zero is accepted and means the line
    /// was removed or reversed: it stays on the document with a total of zero
    /// </summary>
    [HttpPut("{id:long}/items/{itemId:long}")]
    [ProducesResponseType(typeof(InvoiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<InvoiceItemResponse> UpdateItem(long id, long itemId, [FromBody] InvoiceItemRequest request)
    {
        if (findItem(id, itemId) is null) return NotFound();

        var item = request.ToTable(itemId, id);
        Manager.CreateUpdateInvoiceItem(item);

        return InvoiceItemResponse.From(item);
    }

    /// <summary>
    /// Deletes one item of an invoice. Unlike the invoice itself, which is only ever hidden,
    /// an item is a detail of the description and this is a real delete
    /// </summary>
    [HttpDelete("{id:long}/items/{itemId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteItem(long id, long itemId)
    {
        if (findItem(id, itemId) is null) return NotFound();

        Manager.DeleteInvoiceItem(itemId);
        return NoContent();
    }

    private Tables.InvoiceItem? findItem(long invoiceId, long itemId)
    {
        var item = Manager.GetInvoiceItemById(itemId);
        // An item of another invoice is not addressable through this route
        return item?.InvoiceId == invoiceId ? item : null;
    }
}
