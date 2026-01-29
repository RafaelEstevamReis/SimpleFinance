namespace Simple.Finance;

using System;
using System.Collections.Generic;
using System.Linq;

public static class ManagerExtensions
{
    /* Transactions */
    public static IEnumerable<Tables.Transac> GetTransactionsOf(this Manager mgr, Tables.Wallet wallet, Manager.SearchTransactionsDate dateType, DateTime start, DateTime end)
        => mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Wallet, wallet.Id, dateType, start, end);
    public static IEnumerable<Tables.Transac> GetTransactionsOf(this Manager mgr, Tables.Category category, Manager.SearchTransactionsDate dateType, DateTime start, DateTime end)
        => mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Category, category.Id, dateType, start, end);
    public static IEnumerable<Tables.Transac> GetTransactionsOf(this Manager mgr, Tables.Person counterparty, Manager.SearchTransactionsDate dateType, DateTime start, DateTime end)
        => mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Counterparty, counterparty.Id, dateType, start, end);

    /* Dictionaries */
    public static Dictionary<long, Tables.Category> GetCategoriesDict(this Manager mgr)
        => mgr.GetCategories().ToDictionary(o => o.Id, o => o);
    public static Dictionary<long, Tables.Wallet> GetWalletsDict(this Manager mgr)
        => mgr.GetWallets().ToDictionary(o => o.Id, o => o);
    public static Dictionary<long, Tables.Person> GetPersonsDict(this Manager mgr)
        => mgr.GetAllPersons().ToDictionary(o => o.Id, o => o);

    public static (Tables.Transac soruce, Tables.Transac destination) GetTransferPair(this Manager mgr, Tables.Transac oneTransaction)
    {
        if (oneTransaction.Type != Tables.Transac.TransactionType.WalletTransfer)
        {
            throw new ArgumentException("Invalid transaction type", nameof(oneTransaction));
        }

        var otherTransaction = mgr.GetTransactionById(oneTransaction.TypeOtherId);
        if (otherTransaction is null)
        {
            throw new Exception("Invalid 'TypeOtherId' transaction");
        }

        var source = oneTransaction.DueValue < 0 ? oneTransaction : otherTransaction;
        var destination = oneTransaction.DueValue > 0 ? oneTransaction : otherTransaction;

        return (source, destination);
    }

}
