namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.DTOs;
using System;
using System.Linq;

/// <summary>
/// Transactions: the actual money movements
/// </summary>
public class TransactionsController : AccountControllerBase
{
    public TransactionsController(ManagerCache managers) : base(managers) { }

    /// <summary>
    /// Searches transactions in a period.
    /// 'dateType' picks which date rules the search: DueDate for what is owed,
    /// PaymentDate for cash actually moved, EffectiveDate for the mixed timeline.
    /// 'kind' and 'kindId' narrow it to one wallet, category or counterparty;
    /// to cut by more than one of them at a time use /api/transactions/by.
    /// 'order' sorts by that same date, 'limit' keeps only the first rows of that order
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TransactionResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TransactionResponse[]> Search([FromQuery] Manager.SearchTransactionsDate dateType,
                                                      [FromQuery] DateTime start,
                                                      [FromQuery] DateTime end,
                                                      [FromQuery] Manager.SearchTransactionsByKind? kind,
                                                      [FromQuery] long? kindId,
                                                      [FromQuery] SearchOrder? order,
                                                      [FromQuery] int? limit)
    {
        if (kind.HasValue != kindId.HasValue) return BadRequest("'kind' and 'kindId' must be used together");

        // order and limit mean the same thing on both searches, so they are decided in one place
        var shaping = new TransactionSearchRequest { DateType = dateType, Order = order, Limit = limit };
        var rejection = shaping.Rejection();
        if (rejection is not null) return BadRequest(rejection);

        var found = kind.HasValue
            ? Manager.GetTransactionsBy(kind.Value, kindId!.Value, dateType, start, end)
            : Manager.GetTransactions(dateType, start, end);

        return shaping.Shape(found).Select(TransactionResponse.From).ToArray();
    }

    /// <summary>
    /// Searches transactions in a period, narrowed by any combination of ids.
    /// 'walletId', 'categoryId' and 'counterpartyId' are optional and they compose:
    /// every one sent must match, so sending two answers the intersection, never the union.
    /// Leave one out to not filter by it; send 0 to select the rows that carry none of it
    /// </summary>
    [HttpGet("by")]
    [ProducesResponseType(typeof(TransactionResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TransactionResponse[]> SearchBy([FromQuery] TransactionSearchRequest request)
    {
        var rejection = request.Rejection();
        if (rejection is not null) return BadRequest(rejection);

        var found = Manager.GetTransactionsBy(request.WalletId,
                                              request.CategoryId,
                                              request.CounterpartyId,
                                              request.DateType,
                                              request.Start,
                                              request.End);

        return request.Shape(found).Select(TransactionResponse.From).ToArray();
    }

    /// <summary>
    /// A single transaction
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TransactionResponse> Get(long id)
    {
        var tx = Manager.GetTransactionById(id);
        if (tx is null) return NotFound();

        return TransactionResponse.From(tx);
    }

    /// <summary>
    /// Creates a transaction. The sign of the values comes from the category,
    /// so positive values are fine
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TransactionResponse> Create([FromBody] TransactionRequest request)
    {
        var tx = request.ToTable(0);
        Manager.CreateUpdateTransaction(tx);

        return CreatedAtAction(nameof(Get), new { id = tx.Id }, TransactionResponse.From(tx));
    }

    /// <summary>
    /// Updates a transaction. Legs of a wallet transfer cannot be updated here
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TransactionResponse> Update(long id, [FromBody] TransactionRequest request)
    {
        var current = Manager.GetTransactionById(id);
        if (current is null) return NotFound();
        if (current.Type == Tables.Transac.TransactionType.WalletTransfer)
        {
            return BadRequest($"Transaction {id} is a wallet transfer, update it through /api/transfers/{id}");
        }

        var tx = request.ApplyTo(current);
        Manager.CreateUpdateTransaction(tx);

        return TransactionResponse.From(tx);
    }
}
