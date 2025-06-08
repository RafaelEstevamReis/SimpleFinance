namespace Simple.Finance;

using System;
using System.Collections.Generic;

public static class ManagerExtensions
{
    /* Transactions */
    public static IEnumerable<Tables.Transac> GetTransactionsOf(this Manager mgr, Tables.Wallet wallet, Manager.SearchTransactionsDate dateType, DateTime start, DateTime end)
        => mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Wallet, wallet.Id, dateType, start, end);
    public static IEnumerable<Tables.Transac> GetTransactionsOf(this Manager mgr, Tables.Category category, Manager.SearchTransactionsDate dateType, DateTime start, DateTime end)
        => mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Category, category.Id, dateType, start, end);
    public static IEnumerable<Tables.Transac> GetTransactionsOf(this Manager mgr, Tables.Person counterparty, Manager.SearchTransactionsDate dateType, DateTime start, DateTime end)
        => mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Counterparty, counterparty.Id, dateType, start, end);
}
