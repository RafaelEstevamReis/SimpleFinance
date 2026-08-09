namespace Simple.Finance.Importers;

using Simple.Finance.Importers.MT940;
using Simple.Finance.Importers.OFX;
using Simple.Finance.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

public static class TransactionImporter
{
    public static IEnumerable<Transac> FromOFX(string filePath, long walletId, long defaultIncomeCategoryId, long defaultExpenseCategoryId)
    {
        var ofx = OfxFile.FromFile(filePath);
        if (ofx == null) return [];
        return FromOFX(ofx, walletId, defaultIncomeCategoryId, defaultExpenseCategoryId);
    }

    public static IEnumerable<Transac> FromOFX(OfxFile ofx, long walletId, long defaultIncomeCategoryId, long defaultExpenseCategoryId)
    {
        var acc = ofx.GetAllAccountTransactions();
        var credit = ofx.GetAllCreditTransactions();

        foreach (var tx in acc.Union(credit))
        {
            var tr = new Transac
            {
                Id = 0,
                Type = Transac.TransactionType.Simple,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                WalletId = walletId,
                CategoryId = tx.Ammount < 0 ? defaultExpenseCategoryId : defaultIncomeCategoryId,
                CounterpartyId = 0,
                Status = Transac.PaymentStatus.Paid,

                Description = (tx.Name ?? tx.Memo ?? "[?]") + ($" [{tx.FitId}]").Replace("[]", "").Trim(),
                DueDate = tx.DatePosted() ?? DateTime.UtcNow,
                DueValue = tx.Ammount,
                PaymentDate = tx.DatePosted() ?? DateTime.UtcNow,
                PaidValue = tx.Ammount,
                ExternalIdentifier = tx.FitId,
            };

            yield return tr;
        }
    }

    public static IEnumerable<Transac> FromMT940(MT940Statement mt, long walletId, long defaultIncomeCategoryId, long defaultExpenseCategoryId)
    {
        foreach (var tx in mt.Statement)
        {
            int sign = tx.CreditDebitMark == "D" ? -1 : 1;
            decimal value = tx.Amount * sign;
            var tr = new Transac
            {
                Id = 0,
                Type = Transac.TransactionType.Simple,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                WalletId = walletId,
                CategoryId = value < 0 ? defaultExpenseCategoryId : defaultIncomeCategoryId,
                CounterpartyId = 0,
                Status = Transac.PaymentStatus.Paid,

                Description = tx.ReferenceForOwner,
                DueDate = tx.Date,
                DueValue = value,
                PaymentDate = tx.Date,
                PaidValue = value,
            };

            yield return tr;
        }
    }

    public static IEnumerable<Transac> FromCSV(string filePath, Func<string[], Transac> func, char delimiter = ',')
    {
        var lines = DatabaseWrapper.Parsers.CsvParser.ParseCsvFile(filePath, delimiter: delimiter);

        foreach (var line in lines)
        {
            var t = func(line);
            yield return t;
        }
    }
}
