namespace Simple.Finance.Importers.MT940;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MT940Reader
{
    // https://www2.swift.com/knowledgecentre/publications/us9m_20230720/?topic=mt940-field-spec.htm
    static readonly Dictionary<string, string> dicRules = new Dictionary<string, string>
    {
         { "20", "16x" },
         { "21", "16x" },
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
    public static MT940Record ParseMT940RecordsFromFile(string file)
    {
        var lines = File.ReadAllLines(file);
        return ParseMT940RecordsFromLines(lines);
    }
    public static MT940Record ParseMT940RecordsFromLines(string[] lines)
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
                case "60F":
                case "60M":
                    mt940.R60OpeningBalance = r;
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

    public record MTRecord
    {
        public string Field { get; set; } = string.Empty;
        public string OriginalContent { get; set; } = string.Empty;
        public string[] Data { get; set; } = [];
        public MTHelper.Token[] Tokens { get; set; } = [];
    }
    public record MT940Record
    {
        public MTRecord R20TransactionReferenceNumber { get; set; }
        public MTRecord? R21RelatedReference { get; set; }
        public MTRecord R25AccountIdentification { get; set; }
        public MTRecord R28CStatementNumberSequence { get; set; }
        public MTRecord R60OpeningBalance { get; set; }
        public List<(MTRecord, List<MTRecord>)> R61R86StatementeLineInformationAccountOwner { get; set; } = [];
        public MTRecord R62ClosingBalance { get; set; }
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
    public MTOpeningBalance? OpeningBalance { get; set; }
    public MTStatement[] Statement { get; set; } = [];
    public MTClosingBalance? ClosingBalance { get; set; }
    public MTClosingAvailableBalance? ClosingAvailableBalance { get; set; }
    public MTFowardAvailableBalance? FowardAvailableBalance { get; set; }
    public MTInformationToAccountOwner? InformationToAccountOwner { get; set; }

    public class MTTransacationReferenceNumber
    {
        internal static MTTransacationReferenceNumber? Parse(MT940Reader.MTRecord r20TransactionReferenceNumber)
        {
            if (r20TransactionReferenceNumber == null) return null;

            return null;
        }
    }
    public class MTRelatedReference
    {
        internal static MTRelatedReference? Parse(MT940Reader.MTRecord? r21RelatedReference)
        {
            if (r21RelatedReference == null) return null;

            return null;
        }
    }
    public class MTAccountIdentification
    {
        internal static MTAccountIdentification? Parse(MT940Reader.MTRecord r25AccountIdentification)
        {
            if (r25AccountIdentification == null) return null;

            return null;
        }
    }
    public class MTStatementNumberSequence
    {
        internal static MTStatementNumberSequence? Parse(MT940Reader.MTRecord r28CStatementNumberSequence)
        {
            if (r28CStatementNumberSequence == null) return null;

            return null;
        }
    }
    public class MTOpeningBalance
    {
        internal static MTOpeningBalance? Parse(MT940Reader.MTRecord r60OpeningBalance)
        {
            if (r60OpeningBalance == null) return null;

            return null;
        }
    }
    public class MTStatement
    {
        internal static MTStatement[] Parse(List<(MT940Reader.MTRecord, List<MT940Reader.MTRecord>)> r61R86StatementeLineInformationAccountOwner)
        {
            if (r61R86StatementeLineInformationAccountOwner == null) return [];
            if (r61R86StatementeLineInformationAccountOwner.Count == 0) return [];

            return null;
        }
    }
    public class MTClosingBalance
    {
        internal static MTClosingBalance? Parse(MT940Reader.MTRecord r62ClosingBalance)
        {
            if (r62ClosingBalance == null) return null;

            return null;
        }
    }
    public class MTClosingAvailableBalance
    {
        internal static MTClosingAvailableBalance? Parse(MT940Reader.MTRecord? r64ClosingAvailableBalance)
        {
            if (r64ClosingAvailableBalance == null) return null;

            return null;
        }
    }
    public class MTFowardAvailableBalance
    {
        internal static MTFowardAvailableBalance? Parse(MT940Reader.MTRecord? r65FowardAvailableBalance)
        {
            if (r65FowardAvailableBalance == null) return null;

            return null;
        }
    }
    public class MTInformationToAccountOwner
    {
        internal static MTInformationToAccountOwner? Parse(List<MT940Reader.MTRecord> r86InformationToAccountOwner)
        {
            if (r86InformationToAccountOwner == null) return null;

            return null;
        }
    }

}