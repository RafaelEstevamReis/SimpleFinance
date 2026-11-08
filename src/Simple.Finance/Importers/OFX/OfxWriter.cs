namespace Simple.Finance.Importers.OFX;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// Simplified OFX Writer
/// </summary>
public class OfxWriter
{
    public static void WriteOfx(SimplifiedOFX ofx, Stream outputStream)
    {
        if (ofx == null) throw new ArgumentNullException(nameof(ofx));
        if (outputStream == null || !outputStream.CanWrite)
            throw new ArgumentException("Stream not writable", nameof(outputStream));

        if (!ofx.Transactions.Any())
            throw new InvalidOperationException("OFX must contain at least one transaction");

        using var writer = new StreamWriter(outputStream, Encoding.ASCII, 512, leaveOpen: true);

        var referenceDate = ofx.Transactions.Max(t => t.Date);
        // === Header ===
        writer.WriteLine("OFXHEADER:100");
        writer.WriteLine("DATA:OFXSGML");
        writer.WriteLine("VERSION:102");
        writer.WriteLine("SECURITY:NONE");
        writer.WriteLine("ENCODING:USASCII");
        writer.WriteLine("CHARSET:1252");
        writer.WriteLine("COMPRESSION:NONE");
        writer.WriteLine("OLDFILEUID:NONE");
        writer.WriteLine($"NEWFILEUID:NONE");
        writer.WriteLine();
        // === SGML Start ===
        writer.WriteLine("<OFX>");
        writer.WriteLine("<SIGNONMSGSRSV1>");
        writer.WriteLine("<SONRS>");
        writer.WriteLine("<STATUS>");
        writer.WriteLine("<CODE>0</CODE>");
        writer.WriteLine("<SEVERITY>INFO</SEVERITY>");
        writer.WriteLine("</STATUS>");
        writer.WriteLine($"<DTSERVER>{referenceDate:yyyyMMddHHmmss}</DTSERVER>");
        writer.WriteLine("<LANGUAGE>POR</LANGUAGE>");
        writer.WriteLine("</SONRS>");
        writer.WriteLine("</SIGNONMSGSRSV1>");

        writer.WriteLine("<BANKMSGSRSV1>");
        writer.WriteLine("<STMTTRNRS>");
        writer.WriteLine($"<TRNUID>{referenceDate:yyyyMMdd}</TRNUID>");
        writer.WriteLine("<STATUS>");
        writer.WriteLine("<CODE>0</CODE>");
        writer.WriteLine("<SEVERITY>INFO</SEVERITY>");
        writer.WriteLine("</STATUS>");

        writer.WriteLine("<STMTRS>");
        writer.WriteLine("<CURDEF>BRL</CURDEF>");

        writer.WriteLine("<BANKACCTFROM>");
        writer.WriteLine($"<BANKID>{Escape(ofx.BankId)}</BANKID>");
        writer.WriteLine($"<BRANCHID>{Escape(ofx.BranchId)}</BRANCHID>");
        writer.WriteLine($"<ACCTID>{Escape(ofx.AccountId)}</ACCTID>");
        writer.WriteLine($"<ACCTTYPE>{ofx.AccountType}</ACCTTYPE>");
        writer.WriteLine("</BANKACCTFROM>");

        // === TX ===
        var firstDate = ofx.Transactions.Min(t => t.Date);
        var lastDate = ofx.Transactions.Max(t => t.Date);

        writer.WriteLine("<BANKTRANLIST>");
        writer.WriteLine($"<DTSTART>{firstDate:yyyyMMdd}000000</DTSTART>");
        writer.WriteLine($"<DTEND>{lastDate:yyyyMMdd}235959</DTEND>");

        foreach (var t in ofx.Transactions.OrderBy(x => x.Date))
        {
            string type = t.Value >= 0 ? "CREDIT" : "DEBIT";
            string amount = t.Value.ToString("0.00", CultureInfo.InvariantCulture);

            writer.WriteLine("<STMTTRN>");
            writer.WriteLine($"<TRNTYPE>{type}</TRNTYPE>");
            writer.WriteLine($"<DTPOSTED>{t.Date:yyyyMMdd}</DTPOSTED>");
            writer.WriteLine($"<TRNAMT>{amount}</TRNAMT>");
            writer.WriteLine($"<FITID>{Escape(t.Id)}</FITID>");
            writer.WriteLine($"<MEMO>{Escape(t.Memo)}</MEMO>");
            writer.WriteLine("</STMTTRN>");
        }
        writer.WriteLine("</BANKTRANLIST>");

        // === Balances ===
        writer.WriteLine("<LEDGERBAL>");
        writer.WriteLine($"<BALAMT>{ofx.FinalBalance:0.00}</BALAMT>");
        writer.WriteLine($"<DTASOF>{referenceDate:yyyyMMddHHmmss}</DTASOF>");
        writer.WriteLine("</LEDGERBAL>");

        writer.WriteLine("</STMTRS>");
        writer.WriteLine("</STMTTRNRS>");
        writer.WriteLine("</BANKMSGSRSV1>");
        writer.WriteLine("</OFX>");

        writer.Flush();
    }
    private static string Escape(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }

    public class OFXTransaction
    {
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
    }

    public class SimplifiedOFX
    {
        public decimal InitialBalance { get; set; }
        public decimal FinalBalance { get; set; }
        public List<OFXTransaction> Transactions { get; set; } = new();

        public string BankId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string AccountType { get; set; } = "CHECKING";
    }

}
