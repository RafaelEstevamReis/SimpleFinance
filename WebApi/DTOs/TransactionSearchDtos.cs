namespace Simple.Finance.WebApi.DTOs;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A transaction search: a date window, which date rules it, and the ids that narrow it.
/// The ids combine — every one sent must match — so 'walletId' plus 'categoryId' is that
/// category on that wallet, never the union of the two
/// </summary>
public record TransactionSearchRequest
{
    /// <summary>
    /// Which date the window applies to
    /// </summary>
    [FromQuery(Name = "dateType")]
    public Manager.SearchTransactionsDate DateType { get; set; }
    [FromQuery(Name = "start")]
    public DateTime Start { get; set; }
    [FromQuery(Name = "end")]
    public DateTime End { get; set; }

    /// <summary>
    /// Wallet, omit it for every wallet
    /// </summary>
    [FromQuery(Name = "walletId")]
    public long? WalletId { get; set; }
    /// <summary>
    /// Category, omit it for every category. 0 selects the rows that carry none, which are
    /// the legs of a wallet transfer
    /// </summary>
    [FromQuery(Name = "categoryId")]
    public long? CategoryId { get; set; }
    /// <summary>
    /// Counterparty, omit it for anyone. 0 selects the rows with nobody on the other side
    /// </summary>
    [FromQuery(Name = "counterpartyId")]
    public long? CounterpartyId { get; set; }

    /// <summary>
    /// Sorts by the same date 'dateType' chose, not a second one
    /// </summary>
    [FromQuery(Name = "order")]
    public SearchOrder? Order { get; set; }
    /// <summary>
    /// Keeps the first rows of that order, so it needs an 'order' to mean anything
    /// </summary>
    [FromQuery(Name = "limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// What makes the request unanswerable, null when nothing does
    /// </summary>
    public string? Rejection()
    {
        if (Limit.HasValue && Limit.Value < 1) return "'limit' must be at least 1";
        // without an order the rows kept would be whatever the database happened to return first
        if (Limit.HasValue && !Order.HasValue) return "'limit' must be used with an 'order'";

        return null;
    }

    /// <summary>
    /// Applies this request's order and limit to rows the Manager already filtered
    /// </summary>
    public IEnumerable<Tables.Transac> Shape(IEnumerable<Tables.Transac> rows)
    {
        if (Order.HasValue)
        {
            // same date the search filtered by; Id breaks ties so a 'limit' is reproducible
            Func<Tables.Transac, DateTime> date = DateType switch
            {
                Manager.SearchTransactionsDate.DueDate => o => o.DueDate,
                Manager.SearchTransactionsDate.PaymentDate => o => o.PaymentDate,
                Manager.SearchTransactionsDate.Created => o => o.Created,
                Manager.SearchTransactionsDate.Changed => o => o.Changed,
                Manager.SearchTransactionsDate.EffectiveDate => o => o.EffectiveDate,
                _ => throw new InvalidOperationException("Invalid date type"),
            };

            rows = Order.Value == SearchOrder.Desc
                ? rows.OrderByDescending(date).ThenByDescending(o => o.Id)
                : rows.OrderBy(date).ThenBy(o => o.Id);
        }
        if (Limit.HasValue) rows = rows.Take(Limit.Value);

        return rows;
    }
}
