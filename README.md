# Simple.Finance
[![.NET](https://github.com/RafaelEstevamReis/SimpleFinance/actions/workflows/dotnet.yml/badge.svg)](https://github.com/RafaelEstevamReis/SimpleFinance)
[![NuGet](https://img.shields.io/nuget/v/Simple.Finance)](https://www.nuget.org/packages/Simple.Finance)

A simple personal finance manager library with a DEMO Application in WinForms

## How to Use

1. Create the object

~~~C#
// Create a new finance manager
var mgr = new Simple.Finance.Manager("data.db");
~~~

2. Create Wallets and Categories

~~~C#
// Create a new finance manager
var walletId = mgr.CreateUpdateWallet(new Simple.Finance.Tables.Wallet()
{
    Id = 0,
    Name = "Checking Account",
    Description = "",
    IsDeleted = false,
});
var categoryId = mgr.CreateUpdateCategory(new Simple.Finance.Tables.Category()
{
    Id = 0,
    Name = "My House Expenses",
    Description = "",
    IsExpense = true,
    IsDeleted = false,
});
~~~

3. Create your transactions

~~~
long txId = mgr.CreateUpdateTransaction(new Simple.Finance.Tables.Transac
{
    Id = 0,
    CategoryId = categoryId,
    WalletId = walletId,
    Description = $"My Expense Of {DateTime.Now}",
    DueValue = txVal,
    PaidValue = txVal,
    Status = Simple.Finance.Tables.Transac.PaymentStatus.Paid,
    DueDate = DateTime.Now,
    PaymentDate = DateTime.Now,
});
~~~

4. See your transactions

~~~
var recent = mgr.GetTransactionsBy(Manager.SearchTransactionsByKind.Wallet, walletId, 
                                   Manager.SearchTransactionsDate.PaymentDate, DateTime.Now.AddYears(-1), DateTime.Now.AddDays(1))
                .OrderBy(o => o.PaymentDate)
                .TakeLast(10)
                .ToArray();
~~~

## Full history and logs

All changes generate logs

~~~
var recentEvents = mgr.GetLogs(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow)
                      .TakeLast(10);
~~~

## Extendable

Create your derived class and implement your custom needs as users, authentication, etc