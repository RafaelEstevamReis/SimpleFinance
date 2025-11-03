namespace Simple.Finance.Importers.OFX;

using System;
using System.Collections.Generic;
using System.Linq;

/*
public class OFX
{
    private OFX() { }

    public ContaCorrente Conta { get; set; }
    public CartaoCredito Cartao { get; set; }

    public static OFX FromOfxFile(OfxFile File)
    {
        if (File == null) return null;

        OFX ofx = new OFX();
        if (File.AccountInfo != null)
        {
            ofx.Conta = ContaCorrente.FromOfxFile(File.AccountInfo);
        }
        if (File.CreditInfo != null)
        {
            ofx.Cartao = CartaoCredito.FromOfxFile(File.CreditInfo);
        }
        return ofx;
    }
    public static OFX FromTransactionList(IEnumerable<Transacao> lst)
    {
        var trArray = lst.ToArray();
        return new OFX
        {
            Conta = new ContaCorrente()
            {
                PeriodoInicialTransacoes = trArray.Min(o => o.Data),
                PeriodoFinalTransacoes = trArray.Max(o => o.Data),
                Transacoes = trArray,
                Saldo = new Saldo()
                {
                    Data = trArray.Max(o => o.Data),
                    Valor = 0,
                }
            },
        };
    }
}
public class ContaCorrente : ContaMovimentos
{
    internal ContaCorrente() { }
    internal static ContaCorrente FromOfxFile(OfxFile.BankMsg bankMsg)
    {
        if (bankMsg == null) return null;
        if (bankMsg.StmtTrnRs == null) return null;
        if (bankMsg.StmtTrnRs.StmtRs == null) return null;
        ContaCorrente conta = new ContaCorrente();
        ContaMovimentos.preencheDados(conta, bankMsg.StmtTrnRs.StmtRs);
        return conta;
    }
}
public class CartaoCredito : ContaMovimentos
{
    internal CartaoCredito() { this.TipoConta = "CREDIT"; }
    internal static CartaoCredito FromOfxFile(OfxFile.CredMsg credMsg)
    {
        if (credMsg == null) return null;
        if (credMsg.CstmtTrnRs == null) return null;
        if (credMsg.CstmtTrnRs.StmtRs == null) return null;

        CartaoCredito cartao = new CartaoCredito();
        ContaMovimentos.preencheDados(cartao, credMsg.CstmtTrnRs.StmtRs);
        return cartao;
    }
}
public class ContaMovimentos
{
    protected ContaMovimentos() { }

    public string IDBanco { get; set; }
    public string IDConta { get; set; }
    public string TipoConta { get; set; }

    public Saldo Saldo { get; set; }

    public DateTime PeriodoInicialTransacoes { get; set; }
    public DateTime PeriodoFinalTransacoes { get; set; }
    public Transacao[] Transacoes { get; set; }

    protected static void preencheDados(ContaMovimentos mov, OfxFile.StmtRs stmtRs)
    {
        if (stmtRs.BankAccount != null)
        {
            mov.IDBanco = stmtRs.BankAccount.BankId;
            mov.IDConta = stmtRs.BankAccount.AccountId;
            mov.TipoConta = stmtRs.BankAccount.AccountType;
        }
        if (stmtRs.CredAccount != null)
        {
            mov.IDBanco = stmtRs.CredAccount.BankId;
            mov.IDConta = stmtRs.CredAccount.AccountId;
            if (!string.IsNullOrEmpty(stmtRs.CredAccount.AccountType))
                mov.TipoConta = stmtRs.CredAccount.AccountType;
        }

        if (stmtRs.LedgeBalance != null)
        {
            mov.Saldo = new Saldo();
            DateTime dtRef;
            if (OfxFile.TryParseDate(stmtRs.LedgeBalance.DtAsOf, out dtRef))
            {
                mov.Saldo.Data = dtRef;
            }
            else mov.Saldo.Data = DateTime.MinValue;

            // O Saldo é foda ... ...
            string sSaldo = stmtRs.LedgeBalance.Ammount;
            int idxPonto = sSaldo.IndexOf(".");
            int idxVirgula = sSaldo.IndexOf(",");

            // Ajusta para PONTO APENAS
            if (idxPonto >= 0 && idxVirgula >= 0)
            {
                if (idxPonto < idxVirgula) // BR
                {
                    // Inter: 1.207,04
                    sSaldo = sSaldo.Replace(".", "") // Remove separador milhar
                                   .Replace(",", "."); // ajusta separador decimal
                }
                else
                {
                    // US: 1,207.04
                    sSaldo = sSaldo.Replace(",", ""); // Remove separador milhar
                }
            }
            else if (idxPonto >= 0)
            {
                // Está correto
            }
            else // Virgula
            {
                sSaldo = sSaldo.Replace(",", ".");
            }

            decimal.TryParse(sSaldo, out decimal result);
            mov.Saldo.Valor = result;
        }

        mov.PeriodoInicialTransacoes = DateTime.MinValue;
        mov.PeriodoFinalTransacoes = DateTime.MinValue;
        if (mov.Saldo != null)
        {
            mov.PeriodoInicialTransacoes =
                mov.PeriodoFinalTransacoes = mov.Saldo.Data;
        }

        if (stmtRs.TransactionsList != null)
        {
            mov.Transacoes = Transacao.carregaDados(stmtRs.TransactionsList);

            if (mov.Transacoes != null && mov.Transacoes.Length > 0)
            {
                mov.PeriodoInicialTransacoes = mov.Transacoes.Min(o => o.Data);
                mov.PeriodoFinalTransacoes = mov.Transacoes.Max(o => o.Data);
            }

            DateTime dtRef;
            if (OfxFile.TryParseDate(stmtRs.TransactionsList.StartDate, out dtRef))
            {
                mov.PeriodoInicialTransacoes = dtRef;
            }
            if (OfxFile.TryParseDate(stmtRs.TransactionsList.EndDate, out dtRef))
            {
                mov.PeriodoFinalTransacoes = dtRef;
            }
        }
    }
}
public class Saldo
{
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
}
public class Transacao
{
    public string Tipo { get; set; }
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string FitId { get; set; }
    public string RefNum { get; set; }
    public string CheckNum { get; set; }
    public string Memo { get; set; }
    public string Name { get; set; }
    public string Descritivo
    {
        get
        {
            if (Name != null) return Name;
            if (Memo != null) return Memo;
            if (RefNum != null) return RefNum;
            if (RefNum != null) return RefNum;
            if (FitId != null) return FitId;
            return string.Empty;
        }
    }

    internal static Transacao[] carregaDados(OfxFile.BankTransactionList bankTransactionList)
    {
        if (bankTransactionList == null) return null;
        if (bankTransactionList.Transactions == null) return null;
        if (bankTransactionList.Transactions.Length == 0) return new Transacao[0];

        return bankTransactionList.Transactions
            .Where(t => !string.IsNullOrEmpty(t.DtPosted))
            .Select(t => new Transacao()
            {
                Tipo = t.TransactionType,
                Data = OfxFile.ParseDate(t.DtPosted),
                Valor = t.Ammount,
                FitId = t.FitId,
                RefNum = t.RefNum,
                CheckNum = t.CheckNum,
                Memo = t.Memo,
                Name = t.Name
            })
            .ToArray();
    }
}

*/