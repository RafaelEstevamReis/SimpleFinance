namespace Simple.Finance.Importers.MT940;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public static class MTHelper
{
    // https://www2.swift.com/knowledgecentre/publications/us9m_20230720/?topic=mt940-field-spec.htm
    public static IEnumerable<string> ContentSplitter(string content, Token[] ruleSet)
    {
        int pos = 0;

        //foreach (var token in ruleSet)
        for (int i = 0; i < ruleSet.Length; i++)
        {
            var token = ruleSet[i];

            if (pos > content.Length)
            {
                yield return "";
                continue;
            }

            // === LITERAL (ex: "//") ===
            if (token.Type == FieldType.L && token.Literal != null)
            {
                if (pos + token.MaxLen <= content.Length &&
                    content.Substring(pos, token.MaxLen) == token.Literal)
                {
                    yield return token.Literal;
                    pos += token.MaxLen;
                }
                else
                {
                    yield return "";
                }
                continue;
            }

            // === CAMPOS NORMAIS ===
            int start = pos;
            int count = 0;
            int minLen = token.MinLen;
            int maxLen = token.MaxLen;

            while (pos < content.Length && count < maxLen)
            {
                char c = content[pos];
                var nextToken = i < (ruleSet.Length - 1) ? ruleSet[i + 1] : null;

                bool match = token.Type switch
                {
                    FieldType.N => char.IsDigit(c),
                    FieldType.A => char.IsLetter(c),
                    FieldType.C => char.IsLetterOrDigit(c) || c == ' ',
                    FieldType.D => char.IsDigit(c) || c == ',',
                    FieldType.X => true,
                    _ => false
                };

                if (nextToken != null
                    && nextToken.Type == FieldType.L
                    && nextToken.Literal.StartsWith(c))
                {
                    match = false;
                }

                if (!match) break;

                pos++;
                count++;
            }

            // Validação de tamanho
            bool hasContent = count >= minLen;
            bool sizeOk = token.IsFixedLength ? count == maxLen : count <= maxLen;

            if (hasContent && sizeOk)
            {
                yield return content.Substring(start, count);
            }
            else if (token.IsOptional)
            {
                yield return ""; // Optional
            }
            else
            {
                throw new Exception($"Invalid field for rule {token.OriginalRule}");
            }
        }
    }

    public static IEnumerable<string> RuleSpliter(string rule)
    {
        if (rule.Length < 1)
        {
            yield return rule;
            yield break;
        }
        StringBuilder sbCurrentRule = new(10);

        var prevChar = rule[0];
        sbCurrentRule.Append(prevChar);
        for (int i = 1; i < rule.Length; i++)
        {
            var currChar = rule[i];

            if (currChar == '[')
            {
                // break
                yield return sbCurrentRule.ToString();
                sbCurrentRule.Clear();
            }
            else if ((char.IsLetter(prevChar) || prevChar == ']') && char.IsDigit(currChar))
            {
                // break
                yield return sbCurrentRule.ToString();
                sbCurrentRule.Clear();
            }

            sbCurrentRule.Append(currChar);
            prevChar = currChar;
        }
        yield return sbCurrentRule.ToString();
    }
    public static IEnumerable<Token> Tokenizer(string rule) => Tokenizer(RuleSpliter(rule));
    public static IEnumerable<Token> Tokenizer(IEnumerable<string> ruleParts)
    {
        foreach (var part in ruleParts)
        {
            if (string.IsNullOrWhiteSpace(part)) throw new ArgumentException($"Invalid rule block blank");

            // [abc] → opcional
            bool isOptional = part.StartsWith('[') && part.EndsWith(']');
            string clean = isOptional ? part[1..^1] : part;

            // Literal //
            if (clean.StartsWith("//"))
            {
                yield return new Token(2, FieldType.L, isOptional, true, part, "//");
                clean = clean.Substring(2);
            }
            if (clean.StartsWith("/"))
            {
                yield return new Token(1, FieldType.L, isOptional, true, part, "/");
                clean = clean.Substring(1);
            }

            // Último char é o tipo: n, a, c, d, x
            if (clean.Length < 2) throw new ArgumentException($"Invalid rule block length: '{part}'");
            char typeChar = clean[^1];
            var fieldType = typeChar switch
            {
                'n' => FieldType.N,
                'a' => FieldType.A,
                'c' => FieldType.C,
                'd' => FieldType.D,
                'x' => FieldType.X,
                _ => throw new ArgumentException($"Invalid rule block type: '{part}'"),
            };

            // Tem ! antes do tipo? → tamanho fixo
            bool hasBang = clean[^2] == '!';
            bool isFixed = hasBang;

            int lenEnd = clean.Length - (hasBang ? 2 : 1);
            if (!int.TryParse(clean[..lenEnd], out int maxLen)) throw new ArgumentException($"Invalid rule block size: '{part}'");

            yield return new Token(maxLen, fieldType, isOptional, isFixed, part);
        }
    }

    public static IEnumerable<string[]> ParseLinesToBlocks(string[] lines)
    {
        List<string> currentBlock = [];
        foreach (string line in lines)
        {
            if (line.Length == 0) continue; // Error?

            if (line[0] == '{') continue; // Message
            if (line.StartsWith("-}")) continue; // Message

            if (line[0] == ':') // New Block
            {
                if (currentBlock.Count > 0) yield return currentBlock.ToArray();
                currentBlock.Clear();

                currentBlock.Add(line);
            }
            else
            {
                currentBlock.Add(line);
            }
        }
        if (currentBlock.Count > 0) yield return currentBlock.ToArray();
    }

    public static decimal? DecimalParser(string content)
    {
        if (content.Contains(",")) content = content.Replace(",", ".");
        if (content.EndsWith('.')) content = content + "0";

        if (decimal.TryParse(content, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out decimal d))
        {
            return d;
        }
        return null;
    }

    public enum FieldType { N, A, C, D, X, L }
    public record Token
    {
        public Token(int maxLen, FieldType fieldType, bool isOptional, bool isFixed, string originalRule, string literal = "")
        {
            MaxLen = maxLen;
            Type = fieldType;
            IsOptional = isOptional;
            IsFixedLength = isFixed;
            OriginalRule = originalRule;
            Literal = literal;
            MinLen = IsFixedLength ? maxLen : 1;
        }

        public bool IsOptional { get; set; }
        public bool IsFixedLength { get; set; }
        public int MaxLen { get; set; }
        public int MinLen { get; set; }
        public FieldType Type { get; set; }
        public string OriginalRule { get; set; } = "";
        public string Literal { get; set; } = "";
    }
}
