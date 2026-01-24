namespace Simple.Finance.Helpers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

public static class CurrencyHelpers
{
    public static Dictionary<string, CurrencyData> SystemCurrencyData;
    public static Dictionary<string, CurrencyData> CustomCurrencyData;

    static CurrencyHelpers()
    {
        SystemCurrencyData = getSystemCurrenciesDict();
        CustomCurrencyData = getCustomCurrenciesDict();
    }
    private static Dictionary<string, CurrencyData> getSystemCurrenciesDict()
    {
        var data = new Dictionary<string, CurrencyData>();
        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            if (culture.IsNeutralCulture) continue;

            var region = new RegionInfo(culture.Name);
            var format = culture.NumberFormat;

            data[region.ISOCurrencySymbol] = new CurrencyData
            {
                NumberFormatInfo = format,
                Culture = culture,
            };
        }
        return data;
    }
    private static Dictionary<string, CurrencyData> getCustomCurrenciesDict()
    {
        var data = new Dictionary<string, CurrencyData>
        {
            {
                "BTC",
                new CurrencyData
                {
                    Culture = null,
                    NumberFormatInfo = CreateCustomFormatInfo(".", ",", 8, "₿")
                }
            },
            {
                "SAT",
                new CurrencyData
                {
                    Culture = null,
                    NumberFormatInfo = CreateCustomFormatInfo(".", ",", 0, "₿", 3)
                }
            }
        };

        return data;
    }

    public static CurrencyData? GetCurrencyData(string currencyCode)
    {
        if (CustomCurrencyData.TryGetValue(currencyCode, out CurrencyData data)) return data;
        if (SystemCurrencyData.TryGetValue(currencyCode, out data)) return data;
        return null;
    }

    public static string FormatFor(this decimal? value, string currencyCode)
    {
        if (value is null) return "-";
        var data = GetCurrencyData(currencyCode);
        // If NULL, uses INVARIANT
        if (data == null)
        {
            return value.Value.ToString("C2", CultureInfo.InvariantCulture);
        }
        // If have a specified Culture, use it
        if (data.Culture != null)
        {
            return value.Value.ToString("C", data.Culture);
        }

        // Apply custom format
        return string.Format(data.NumberFormatInfo, "{0:C}", value.Value);
    }

    public static NumberFormatInfo CreateCustomFormatInfo(string decimalSeparator, string groupSeparator, int decimalPlaces, string symbol, int currencyPattern = 2)
    {
        var format = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        format.CurrencyDecimalSeparator = decimalSeparator;
        format.CurrencyGroupSeparator = groupSeparator;
        format.CurrencyDecimalDigits = decimalPlaces;
        format.CurrencySymbol = symbol;
        format.CurrencyPositivePattern = currencyPattern;
        format.CurrencyNegativePattern = 9;
        return format;
    }

    public static string GetSystemCurrencyCode()
    {
        var region = new RegionInfo(CultureInfo.CurrentCulture.Name);
        return region.ISOCurrencySymbol;
    }

    public class CurrencyData
    {
        public CultureInfo? Culture { get; set; }
        public NumberFormatInfo NumberFormatInfo { get; set; } = null!;
    }
}