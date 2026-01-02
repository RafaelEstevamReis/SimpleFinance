namespace Simple.Finance.Importers.OFX;

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;

// https://www.ofx.net/downloads/OFX%202.2.pdf
// https://web.archive.org/web/20210515061017/https://www.ofx.net/downloads/OFX%202.2.pdf

[XmlRoot(Namespace = "", IsNullable = false)]
[XmlType(TypeName = "OFX")]
public class OfxFile
{
    [XmlElement("SIGNONMSGSRSV1")]
    public Fileinfo? FileInfo { get; set; }
    [XmlElement("BANKMSGSRSV1")]
    public BankMsg? AccountInfo { get; set; }
    [XmlElement("CREDITCARDMSGSRSV1")]
    public CredMsg? CreditInfo { get; set; }

    public Transaction[] GetAllAccountTransactions()
    {
        return AccountInfo?.StmtTrnRs?.StmtRs?.TransactionsList?.Transactions ?? [];
    }
    public Transaction[] GetAllCreditTransactions()
    {
        return CreditInfo?.CstmtTrnRs?.StmtRs?.TransactionsList?.Transactions ?? [];
    }

    public static OfxFile? FromFile_Encoding1252(string FilePath) => FromFile(FilePath, Encoding.GetEncoding(1252));
    public static OfxFile? FromFile(string FilePath) => FromFile(FilePath, Encoding.UTF8);
    public static OfxFile? FromFile(string FilePath, Encoding encoding)
    {
        var xml = File.ReadAllText(FilePath, encoding);
        var ofx = FromXML(xml);
        if (ofx?.FileInfo != null) ofx.FileInfo.FileOnDisk = FilePath;
        return ofx;
    }
    public static OfxFile? FromXML(string fileContents)
    {
        if (!fileContents.Contains("<OFX>")) return null;
        fileContents = fileContents.Substring(fileContents.IndexOf("<OFX>"));
        // Converter SGXL to XML
        if (!fileContents.Contains("\n<"))
        {
            fileContents = arrumaSgxl(fileContents);
        }
        else fileContents = arrumaSgxlV2(fileContents);

        byte[] byteArray = Encoding.UTF8.GetBytes(fileContents);
        using MemoryStream stream = new MemoryStream(byteArray);

        return LoadXml<OfxFile>(stream);
    }
    private static string arrumaSgxlV2(string fileContents)
    {
        var lines = fileContents.Replace("\r", "")
                                .Split('\n')
                                .Select(l => l.Trim())
                                .ToArray();

        for (int i = 0; i < lines.Length; i++)
        {
            if (!lines[i].Contains("<")) continue;

            if (lines[i] == "<MEMO>") lines[i] = "<MEMO></MEMO>";
            if (lines[i] == "<BALAMT>") lines[i] = "<BALAMT>0</BALAMT>";
            if (lines[i].Contains(" & ")) lines[i] = lines[i].Replace("& ", "&amp; ");

            if (lines[i].EndsWith(">")) continue;

            string tag = lines[i].Split('>')[0].Substring(1);

            lines[i] += string.Format("</{0}>", tag);
        }
        return string.Join("\n", lines);
    }
    private static string arrumaSgxl(string SgmlContents)
    {
        while (SgmlContents.Contains("> ")) SgmlContents = SgmlContents.Replace("> ", ">");
        while (SgmlContents.Contains(" <")) SgmlContents = SgmlContents.Replace(" <", "<");
        while (SgmlContents.Contains(">\n")) SgmlContents = SgmlContents.Replace(">\n", ">");
        while (SgmlContents.Contains(">\r")) SgmlContents = SgmlContents.Replace(">\r", ">");
        while (SgmlContents.Contains("\n<")) SgmlContents = SgmlContents.Replace("\n<", "<");
        while (SgmlContents.Contains("\r<")) SgmlContents = SgmlContents.Replace("\r<", "<");
        while (SgmlContents.Contains("> ")) SgmlContents = SgmlContents.Replace("> ", ">");
        while (SgmlContents.Contains(" <")) SgmlContents = SgmlContents.Replace(" <", "<");

        // Faz parte a parte para não usar um Sgxml Parser
        string temp = "";

        // Fecha Severity
        if (!SgmlContents.Contains("</CODE>")) SgmlContents = SgmlContents.Replace("<SEVERITY>", "</CODE><SEVERITY>");
        //temp = temp;
        if (!SgmlContents.Contains("</SEVERITY>")) SgmlContents = SgmlContents.Replace("</STATUS>", "</SEVERITY></STATUS>");
        if (!SgmlContents.Contains("</DTSERVER>")) SgmlContents = SgmlContents.Replace("<LANGUAGE>", "</DTSERVER><LANGUAGE>");
        if (!SgmlContents.Contains("</LANGUAGE>")) SgmlContents = SgmlContents.Replace("</SONRS>", "</LANGUAGE></SONRS>");
        // TRNUID é mais complicado, tem uma tag comum antes dele, a "<STATUS>"
        if (!SgmlContents.Contains("</TRNUID>"))
        {
            temp = SgmlContents.Substring(SgmlContents.IndexOf("<TRNUID>", 10));
            temp = temp.Substring(0, temp.IndexOf('<', 3));
            SgmlContents = SgmlContents.Replace(temp, temp + "</TRNUID>");
        }
        if (!SgmlContents.Contains("</CURDEF>")) SgmlContents = SgmlContents.Replace("<BANKACCTFROM>", "</CURDEF><BANKACCTFROM>");
        if (!SgmlContents.Contains("</BANKID>")) SgmlContents = SgmlContents.Replace("<ACCTID>", "</BANKID><ACCTID>");
        if (!SgmlContents.Contains("</ACCTID>")) SgmlContents = SgmlContents.Replace("<ACCTTYPE>", "</ACCTID><ACCTTYPE>");
        if (!SgmlContents.Contains("</ACCTTYPE>")) SgmlContents = SgmlContents.Replace("</BANKACCTFROM>", "</ACCTTYPE></BANKACCTFROM>");
        if (!SgmlContents.Contains("</DTSTART>")) SgmlContents = SgmlContents.Replace("<DTEND>", "</DTSTART><DTEND>");
        if (!SgmlContents.Contains("</DTEND>"))
        {
            temp = SgmlContents.Substring(SgmlContents.IndexOf("<DTEND>", 30));
            temp = temp.Substring(0, temp.IndexOf('<', 3));
            SgmlContents = SgmlContents.Replace(temp, temp + "</DTEND>");
        }
        if (!SgmlContents.Contains("</TRNTYPE>")) SgmlContents = SgmlContents.Replace("<DTPOSTED>", "</TRNTYPE><DTPOSTED>");
        if (!SgmlContents.Contains("</DTPOSTED>")) SgmlContents = SgmlContents.Replace("<TRNAMT>", "</DTPOSTED><TRNAMT>");
        if (!SgmlContents.Contains("</TRNAMT>")) SgmlContents = SgmlContents.Replace("<FITID>", "</TRNAMT><FITID>");
        if (!SgmlContents.Contains("</FITID>")) SgmlContents = SgmlContents.Replace("<CHECKNUM>", "</FITID><CHECKNUM>");
        if (!SgmlContents.Contains("</CHECKNUM>")) SgmlContents = SgmlContents.Replace("<MEMO>", "</CHECKNUM><MEMO>");
        if (!SgmlContents.Contains("</MEMO>")) SgmlContents = SgmlContents.Replace("</STMTTRN>", "</MEMO></STMTTRN>");

        //temp = temp;
        if (!SgmlContents.Contains("</BALAMT>")) SgmlContents = SgmlContents.Replace("<DTASOF>", "</BALAMT><DTASOF>");
        if (!SgmlContents.Contains("</DTASOF>")) SgmlContents = SgmlContents.Replace("</LEDGERBAL>", "</DTASOF></LEDGERBAL>");
        //temp = temp;
        return SgmlContents;
    }

    public static DateTime ParseDate(string Date)
    {
        DateTime dt;
        if (TryParseDate(Date, out dt))
        {
            return dt;
        }
        return DateTime.MinValue;
    }
    public static bool TryParseDate(string StringDate, out DateTime Date)
    {
        Date = DateTime.MinValue;
        if (StringDate == null) return false;
        // Não há um bom parse direto, vou transformar
        string tz = "";
        string raizData = StringDate;

        if (raizData.Contains(".")) raizData = raizData.Split('.')[0];
        if (raizData.Contains("["))
        {
            var p = raizData.Split('[');
            raizData = p[0];
            tz = p[1].Replace("]", "");
        }

        if (tz.Contains(":"))
        {
            int.TryParse(tz.Split(':')[0], out int numTz);

            if (numTz > 0)
                tz = " +" + numTz + ":00";
            else
                tz = " " + numTz + ":00";
        }

        if (raizData.Length == 8)
        {
            raizData = raizData.Substring(0, 4) + "-"
                       + raizData.Substring(4, 2) + "-"
                       + raizData.Substring(6, 2);
        }
        if (raizData.Length == 14)
        {
            raizData = raizData.Substring(0, 4) + "-"
                       + raizData.Substring(4, 2) + "-"
                       + raizData.Substring(6, 2) + " "
                       + raizData.Substring(8, 2) + ":"
                       + raizData.Substring(10, 2) + ":"
                       + raizData.Substring(12, 2);
        }

        return DateTime.TryParse(raizData + tz, out Date);
    }

    public static T LoadXml<T>(Stream source)
    {
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(source);
    }


    [Serializable()]
    public class Status
    {
        [XmlElement("CODE")]
        public int Code { get; set; }
        [XmlElement("SEVERITY")]
        public string? Severity { get; set; }
    }

    #region FileInfo
    [Serializable()]
    public class Fileinfo
    {
        [XmlElement("SONRS")]
        public FileinfoSorns? Sorns { get; set; }
        public string? FileOnDisk { get; set; }
    }
    [Serializable()]
    public class FileinfoSorns
    {
        [XmlElement("STATUS")]
        public Status? FileInfoSornsStatus { get; set; }
        [XmlElement("DTSERVER")]
        public string? DtServer { get; set; }
        [XmlElement("LANGUAGE")]
        public string? Language { get; set; }
    }
    #endregion

    [Serializable()]
    public class BankMsg
    {
        [XmlElement("STMTTRNRS")]
        public StmtTrnRs? StmtTrnRs { get; set; }
    }
    [Serializable()]
    public class CredMsg
    {
        [XmlElement("CCSTMTTRNRS")]
        public CstmtTrnRs? CstmtTrnRs { get; set; }
    }

    [Serializable()]
    public class StmtTrnRs
    {
        [XmlElement("TRNUID")]
        public string? TrnUid { get; set; }
        [XmlElement("STATUS")]
        public Status? Status { get; set; }
        [XmlElement("STMTRS")]
        public StmtRs? StmtRs { get; set; }
    }
    [Serializable()]
    public class CstmtTrnRs
    {
        [XmlElement("TRNUID")]
        public string? TrnUid { get; set; }
        [XmlElement("STATUS")]
        public Status? Status { get; set; }
        [XmlElement("CCSTMTRS")]
        public StmtRs? StmtRs { get; set; }
    }
    [Serializable()]
    public class StmtRs
    {
        [XmlElement("CURDEF")]
        public string? DefaultCurrency { get; set; }
        [XmlElement("BANKACCTFROM")]
        public BankAccount? BankAccount { get; set; }
        [XmlElement("CCACCTFROM")]
        public BankAccount? CredAccount { get; set; }
        [XmlElement("BANKTRANLIST")]
        public BankTransactionList? TransactionsList { get; set; }
        [XmlElement("LEDGERBAL")]
        public LedgeBalance? LedgeBalance { get; set; }
    }
    [Serializable()]
    public class BankAccount
    {
        [XmlElement("BANKID")]
        public string? BankId { get; set; }
        [XmlElement("ACCTID")]
        public string? AccountId { get; set; }
        [XmlElement("ACCTTYPE")]
        public string? AccountType { get; set; }
    }
    [Serializable()]
    public class BankTransactionList
    {
        [XmlElement("DTSTART")]
        public string? EndDate { get; set; }
        [XmlElement("DTEND")]
        public string? StartDate { get; set; }

        public DateTime? dtStartDate()
        {
            if (StartDate == null) return null;
            return DateTime.ParseExact(StartDate, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
        public DateTime? dtEndDate()
        {
            if (EndDate == null) return null;
            return DateTime.ParseExact(EndDate, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        [XmlElement("STMTTRN")]
        public Transaction[] Transactions { get; set; } = [];
    }
    [Serializable()]
    public class Transaction
    {
        [XmlElement("TRNTYPE")]
        public string? TransactionType { get; set; }
        [XmlElement("DTPOSTED")]
        public string? DtPosted { get; set; }
        [XmlElement("TRNAMT")]
        public decimal Ammount { get; set; }
        [XmlElement("FITID")]
        public string? FitId { get; set; }
        [XmlElement("REFNUM")]
        public string? RefNum { get; set; }
        [XmlElement("CHECKNUM")]
        public string? CheckNum { get; set; }
        [XmlElement("MEMO")]
        public string? Memo { get; set; }
        [XmlElement("NAME")]
        public string? Name { get; set; }

        public DateTime? DatePosted()
        {
            if (DtPosted is null) return null;

            if (DtPosted.Length >= 20 && DtPosted.Contains("["))
            {
                return DateTime.ParseExact(DtPosted.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
            }

            if (DateTime.TryParseExact(DtPosted, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dt)) return dt;
            return DateTime.Parse(DtPosted, CultureInfo.InvariantCulture);
        }
    }
    [Serializable()]
    public class LedgeBalance
    {
        [XmlElement("BALAMT")]
        public string? Ammount { get; set; }
        [XmlElement("DTASOF")]
        public string? DtAsOf { get; set; }

        public DateTime DateAsOf()
        {
            if (DtAsOf is null) return DateTime.MinValue;
            return DateTime.ParseExact(DtAsOf, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
        public decimal Amount()
        {
            if (Ammount == null) return 0;
            return Convert.ToDecimal(Ammount, CultureInfo.InvariantCulture);
        }
    }

}