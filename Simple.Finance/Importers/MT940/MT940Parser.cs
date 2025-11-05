namespace Simple.Finance.Importers.MT940;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class MT940Parser
{
    // https://www2.swift.com/knowledgecentre/publications/us9m_20230720/?topic=mt940-field-spec.htm
    static readonly Dictionary<string, string> dicRules = new Dictionary<string, string>
    {
         { "20", "16x" },
         { "21", "16x" },
         { "25", "35x" },
         { "28C", "5n[/5n]" },
         { "60F", "1!a6!n3!a15d" },
         { "60M", "1!a6!n3!a15d" },
         { "61", "6!n[4!n]2a[1!a]15d1!a3!c16x[//16x]" },
         { "62F", "1!a6!n3!a15d" },
         { "62M", "1!a6!n3!a15d" },
         { "64", "1!a6!n3!a15d" },
         { "65", "1!a6!n3!a15d" },
         { "86", "65x" },
    };

    public static MT940Statement FromFile(string file, System.Text.Encoding? encoding)
    {
        var lines = File.ReadAllLines(file, encoding ?? System.Text.Encoding.UTF8);
        return FromLines(lines);
    }
    public static MT940Statement FromLines(string[] lines)
    {
        var record = ParserMT940RecordFromLines(lines);
        return ParseMT940FromRecord(record);
    }

    public static MT940Statement ParseMT940FromRecord(MT940Record record)
    {
        var mt940 = new MT940Statement()
        {
            TransacationReferenceNumber = MT940Statement.MTTransacationReferenceNumber.Parse(record.R20TransactionReferenceNumber),
            RelatedReference = MT940Statement.MTRelatedReference.Parse(record.R21RelatedReference),
            AccountIdentification = MT940Statement.MTAccountIdentification.Parse(record.R25AccountIdentification),
            StatementNumberSequence = MT940Statement.MTStatementNumberSequence.Parse(record.R28CStatementNumberSequence),
            OpeningBalance = MT940Statement.MTBalance.Parse(record.R60FOpeningBalance), // First
            Statement = MT940Statement.MTStatement.Parse(record.R61R86StatementeLineInformationAccountOwner),
            ClosingBalance = MT940Statement.MTBalance.Parse(record.R62ClosingBalance),
            ClosingAvailableBalance = MT940Statement.MTBalance.Parse(record.R64ClosingAvailableBalance),
            FowardAvailableBalance = MT940Statement.MTBalance.Parse(record.R65FowardAvailableBalance),
            InformationToAccountOwner = MT940Statement.MTInformationToAccountOwner.Parse(record.R86InformationToAccountOwner),
        };
        return mt940;
    }

    public static MT940Record ParseMT940RecordFromFile(string file, System.Text.Encoding? encoding)
    {
        var lines = File.ReadAllLines(file, encoding ?? System.Text.Encoding.UTF8);
        return ParserMT940RecordFromLines(lines);
    }
    public static MT940Record ParserMT940RecordFromLines(string[] lines)
    {
        var records = ParseRecordsFromLines(lines);

        var mt940 = new MT940Record();
        bool had62 = false;
        foreach (var r in records)
        {
            switch (r.Field)
            {
                case "20":
                    mt940.R20TransactionReferenceNumber = r;
                    break;
                case "21":
                    mt940.R21RelatedReference = r;
                    break;
                case "25":
                case "25P":
                    mt940.R25AccountIdentification = r;
                    break;
                case "28C":
                    mt940.R28CStatementNumberSequence = r;
                    break;
                case "60F": // First opening balance
                    mt940.R60FOpeningBalance = r;
                    break;
                case "60M": // intermediate opening balance
                    mt940.R60MOpeningBalance.Add(r);
                    break;

                case "61":
                    mt940.R61R86StatementeLineInformationAccountOwner.Add((r, []));
                    break;
                case "86":
                    if (had62) // Put on end
                    {
                        mt940.R86InformationToAccountOwner.Add(r);
                    }
                    else // Put nested on 61
                    {
                        mt940.R61R86StatementeLineInformationAccountOwner[^1].Item2.Add(r);
                    }
                    break;
                case "62F":
                case "62M":
                    mt940.R62ClosingBalance = r;
                    had62 = true;
                    break;
                case "64":
                    mt940.R64ClosingAvailableBalance = r;
                    break;
                case "65":
                    mt940.R65FowardAvailableBalance = r;
                    break;

                default:
                    mt940.Other.Add(r);
#if DEBUG
                    throw new NotImplementedException($"Not supported field: {r.Field}");
#endif
            }
        }

        return mt940;
    }

    public static IEnumerable<MTRecord> ParseRecordsFromFile(string file)
    {
        var lines = File.ReadAllLines(file);
        return ParseRecordsFromLines(lines);
    }
    private static IEnumerable<MTRecord> ParseRecordsFromLines(string[] lines)
    {
        var blocks = MTHelper.ParseLinesToBlocks(lines);
        foreach (var b in blocks)
        {
            var l1 = b[0];
            var field = l1.Split(':')[1];
            var content = l1.Substring(field.Length + 2);

            var record = new MTRecord
            {
                Field = field,
                OriginalContent = string.Join('\n', b),
            };

            if (dicRules.TryGetValue(record.Field, out string rule))
            {
                var tokens = MTHelper.Tokenizer(rule).ToArray();
                var data = MTHelper.ContentSplitter(content, tokens).ToArray();

                record.Tokens = tokens;
                record.Data = data;
            }

            yield return record;
        }
    }

    public record MTRecord
    {
        public string Field { get; set; } = string.Empty;
        public string OriginalContent { get; set; } = string.Empty;
        public string[] Data { get; set; } = [];
        public MTHelper.Token[] Tokens { get; set; } = [];
    }
    public record MT940Record
    {
        public MTRecord? R20TransactionReferenceNumber { get; set; }
        public MTRecord? R21RelatedReference { get; set; }
        public MTRecord? R25AccountIdentification { get; set; }
        public MTRecord? R28CStatementNumberSequence { get; set; }
        public MTRecord? R60FOpeningBalance { get; set; }
        public List<MTRecord> R60MOpeningBalance { get; set; } = [];
        public List<(MTRecord, List<MTRecord>)> R61R86StatementeLineInformationAccountOwner { get; set; } = [];
        public MTRecord? R62ClosingBalance { get; set; }
        public MTRecord? R64ClosingAvailableBalance { get; set; }
        public MTRecord? R65FowardAvailableBalance { get; set; }
        public List<MTRecord> R86InformationToAccountOwner { get; set; } = [];
        public List<MTRecord> Other { get; set; } = [];
    }
}
public class MT940Statement
{
    public MTTransacationReferenceNumber? TransacationReferenceNumber { get; set; }
    public MTRelatedReference? RelatedReference { get; set; }
    public MTAccountIdentification? AccountIdentification { get; set; }
    public MTStatementNumberSequence? StatementNumberSequence { get; set; }
    public MTBalance? OpeningBalance { get; set; } // Can have intermediates
    public MTStatement[] Statement { get; set; } = [];
    public MTBalance? ClosingBalance { get; set; }
    public MTBalance? ClosingAvailableBalance { get; set; }
    public MTBalance? FowardAvailableBalance { get; set; }
    public MTInformationToAccountOwner? InformationToAccountOwner { get; set; }

    public class MTTransacationReferenceNumber
    {
        public string ReferenceId { get; set; } = string.Empty;

        internal static MTTransacationReferenceNumber? Parse(MT940Parser.MTRecord r20TransactionReferenceNumber)
        {
            if (r20TransactionReferenceNumber == null) return null;

            return new MTTransacationReferenceNumber
            {
                ReferenceId = r20TransactionReferenceNumber.Data[0],
            };
        }
    }
    public class MTRelatedReference
    {
        public string ReferenceId { get; set; } = string.Empty;

        internal static MTRelatedReference? Parse(MT940Parser.MTRecord? r21RelatedReference)
        {
            if (r21RelatedReference == null) return null;

            return new MTRelatedReference
            {
                ReferenceId = r21RelatedReference.Data[0]
            };
        }
    }
    public class MTAccountIdentification
    {
        public string AccountId { get; set; } = string.Empty;

        internal static MTAccountIdentification? Parse(MT940Parser.MTRecord r25AccountIdentification)
        {
            if (r25AccountIdentification == null) return null;

            return new MTAccountIdentification
            {
                AccountId = r25AccountIdentification.Data[0],
            };
        }
    }
    public class MTStatementNumberSequence
    {
        public string StatementNumber { get; set; } = string.Empty;
        public string SquenceNumber { get; set; } = string.Empty;

        internal static MTStatementNumberSequence? Parse(MT940Parser.MTRecord r28CStatementNumberSequence)
        {
            if (r28CStatementNumberSequence == null) return null;

            var s = new MTStatementNumberSequence
            {
                StatementNumber = r28CStatementNumberSequence.Data[0],
            };
            // Optional
            if (r28CStatementNumberSequence.Data.Length >= 2) s.SquenceNumber = r28CStatementNumberSequence.Data[2];

            return s;
        }
    }
    public class MTStatement
    {
        public DateTime Date { get; set; }
        public DateTime? EntryDate { get; set; }
        public string CreditDebitMark { get; set; } = string.Empty;
        public string? FundsCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string TransactionCode { get; set; } = string.Empty;
        public string ReferenceForOwner { get; set; } = string.Empty;
        public string ReferenceForServicingInstitution { get; set; } = string.Empty;

        public string[] AdditionalInformation { get; set; } = [];

        internal static MTStatement[] Parse(List<(MT940Parser.MTRecord, List<MT940Parser.MTRecord>)> r61R86StatementeLineInformationAccountOwner)
        {
            if (r61R86StatementeLineInformationAccountOwner == null) return [];
            if (r61R86StatementeLineInformationAccountOwner.Count == 0) return [];

            return r61R86StatementeLineInformationAccountOwner.Select(buildSingle).ToArray();
        }
        private static MTStatement buildSingle((MT940Parser.MTRecord, List<MT940Parser.MTRecord>) block)
        {
            var b61 = block.Item1;
            var arrb86 = block.Item2;

            var markFundsCode = b61.Data[2] + b61.Data[3];

            string cdMark;
            string? fundsCode;
            if (markFundsCode[0] == 'R')
            {
                // exact 2 are from Mark
                if (markFundsCode.Length == 2)
                {
                    cdMark = markFundsCode;
                    fundsCode = null;
                }
                else if (markFundsCode.Length == 3)
                {
                    cdMark = markFundsCode[..2];
                    fundsCode = markFundsCode[^1..];
                }
                else
                {
                    // ?
                    throw new Exception("Invalid Markins for CD/Funds");
                }
            }
            else
            {
                // exact 1 is from Mark
                if (markFundsCode.Length == 1)
                {
                    cdMark = markFundsCode;
                    fundsCode = null;
                }
                else if (markFundsCode.Length == 2)
                {
                    cdMark = markFundsCode[..1];
                    fundsCode = markFundsCode[^1..];
                }
                else
                {
                    // ?
                    throw new Exception("Invalid Markins for CD/Funds");
                }
            }

            return new MTStatement
            {
                Date = DateTime.ParseExact(b61.Data[0], "yyMMdd", CultureInfo.InvariantCulture),
                EntryDate = null, // How can I get the Year? Is aways learlier than Date
                CreditDebitMark = cdMark,
                FundsCode = fundsCode,
                Amount = MTHelper.DecimalParser(b61.Data[4]) ?? 0,
                TransactionType = b61.Data[5],
                TransactionCode = b61.Data[6],
                ReferenceForOwner = b61.Data[7],
                ReferenceForServicingInstitution = b61.Data[9], // [8] is "//"
                AdditionalInformation = arrb86.Select(o => o.Data[0]).ToArray(),
            };
        }
    }
    public class MTInformationToAccountOwner
    {
        internal static MTInformationToAccountOwner? Parse(List<MT940Parser.MTRecord> r86InformationToAccountOwner)
        {
            if (r86InformationToAccountOwner == null
                || r86InformationToAccountOwner.Count == 0) return null;

            return null;
        }
    }

    public class MTBalance
    {
        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public static MTBalance? Parse(MT940Parser.MTRecord? balanceRecord)
        {
            if (balanceRecord == null) return null;

            return new MTBalance
            {
                Type = balanceRecord.Data[0],
                Date = DateTime.ParseExact(balanceRecord.Data[1], "yyMMdd", CultureInfo.InvariantCulture),
                Currency = balanceRecord.Data[2],
                Amount = MTHelper.DecimalParser(balanceRecord.Data[3]) ?? 0,
            };
        }
    }

}
