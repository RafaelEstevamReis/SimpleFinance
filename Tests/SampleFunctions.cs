namespace Tests;

using System;
using System.Linq;

public class SampleFunctions
{
    static Random Rnd = new();
    public static void Run(Simple.Finance.Manager mgr)
    {
        Console.WriteLine(">> Sample Functions\n");

        /* Wallets */
        var wallets = mgr.GetWallets().ToArray();
        if (wallets.Length == 0)
        {
            mgr.CreateUpdateWallet(new Simple.Finance.Tables.Wallet()
            {
                Id = 0,
                Name = "Default Wallet",
                Description = "",
                IsDeleted = false,
            });
            wallets = mgr.GetWallets().ToArray();
        }

        /* Categories */
        var cats = mgr.GetCategories().ToArray();
        if (cats.Length == 0)
        {
            mgr.CreateUpdateCategory(new Simple.Finance.Tables.Category()
            {
                Id = 0,
                Name = "My House Expenses",
                Description = "",
                IsExpense = true,
                IsDeleted = false,
            });
            mgr.CreateUpdateCategory(new Simple.Finance.Tables.Category()
            {
                Id = 0,
                Name = "My Food Expenses",
                Description = "",
                IsExpense = true,
                IsDeleted = false,
            });
            mgr.CreateUpdateCategory(new Simple.Finance.Tables.Category()
            {
                Id = 0,
                Name = "Income",
                Description = "",
                IsExpense = false,
                IsDeleted = false,
            });
            cats = mgr.GetCategories().ToArray();
        }
        Console.WriteLine($"{cats.Length} Categories");
        Console.WriteLine(string.Join("\n", cats.Select(x => $" * {x.Id}: [{(x.IsExpense ? "EXP" : "INC")}] {x.Name}")));

        /* Transactions */
        var txVal = Math.Round((decimal)(Rnd.NextDouble() * 100), 2);
        mgr.CreateUpdateTransaction(new Simple.Finance.Tables.Transac
        {
            Id = 0,
            CategoryId = oneOf(cats.Where(o => o.IsExpense)).Id,
            WalletId = wallets[0].Id,
            Description = $"My Expense Of {DateTime.Now}",
            DueValue = txVal,
            PaidValue = txVal,
            Status = Simple.Finance.Tables.Transac.PaymentStatus.Paid,
            DueDate = DateTime.Now,
            PaymentDate = DateTime.Now,
        });
        if (Rnd.NextDouble() < 0.1)
        {
            txVal = Math.Round((decimal)(250 + Rnd.NextDouble() * 500), 0);
            mgr.CreateUpdateTransaction(new Simple.Finance.Tables.Transac
            {
                Id = 0,
                CategoryId = oneOf(cats.Where(o => !o.IsExpense)).Id,
                WalletId = wallets[0].Id,
                Description = $"Income of {DateTime.Now}",
                DueValue = txVal,
                PaidValue = txVal,
                Status = Simple.Finance.Tables.Transac.PaymentStatus.Paid,
                DueDate = DateTime.Now,
                PaymentDate = DateTime.Now,
            });
        }

        Console.WriteLine($"{wallets.Length} Wallets");
        Console.WriteLine(string.Join("\n", wallets.Select(x => $" * {x.Id}: {x.Name} - Bal: {mgr.GetWalletBalance(x.Id):N2}")));

        var recent = mgr.GetTransactions(Simple.Finance.Manager.SearchTransactionsDate.PaymentDate, DateTime.Now.AddYears(-1), DateTime.Now.AddDays(1))
                        .OrderBy(o => o.PaymentDate)
                        .TakeLast(10)
                        .ToArray();

        decimal total = 0;
        Console.WriteLine("Recent:");
        Console.WriteLine(
                $"* {"Id",-5} " +
                $"{"Category",-20} " +
                $"{"Description",-40} " +
                $"{"Date",-20} " +
                $"{"Value",8:F2} " +
                $"{"Total",8:F2}");
        foreach (var tx in recent)
        {
            total += tx.PaidValue;
            Console.WriteLine(
                $"* {tx.Id,-5} " +
                $"{cats.FirstOrDefault(o => o.Id == tx.CategoryId)?.Name ?? "",-20} " +
                $"{tx.Description,-40} " +
                $"{tx.PaymentDate,-20} " +
                $"{tx.PaidValue,8:F2} " +
                $"{total,8:F2}");
        }


    }
    private static T oneOf<T>(T[] items)
    {
        var ix = Rnd.Next(items.Length);
        return items[ix];
    }
    private static T oneOf<T>(IEnumerable<T> items) => oneOf(items.ToArray());
}
