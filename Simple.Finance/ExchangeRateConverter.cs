namespace Simple.Finance;

using Simple.Finance.ExchangeConverters;
using System;
using System.Collections.Generic;

public class ExchangeRateConverter
{
    Dictionary<string, decimal> dicRate = [];
    public List<IExchangeRateTable> ExchangeRateTables { get; set; } = [];
    
    /// <summary>
    /// Cache recent queries to avoid re-process same pairs on a date
    /// </summary>
    public bool CacheEnabled { get; set; } = true;

    public decimal? GetRateFor(string baseCur, string quoteCur, DateTime dateUTC)
    {
        if (ExchangeRateTables.Count == 0) throw new InvalidOperationException($"There are no {nameof(ExchangeRateTables)} to process");

        string cacheKey = $"{baseCur}/{quoteCur}#{dateUTC:yyyyMMdd}";
        if (CacheEnabled)
        {
            if (dicRate.ContainsKey(cacheKey)) return dicRate[cacheKey];
        }

        foreach (var converter in ExchangeRateTables)
        {
            var rate = converter.GetRateFor(baseCur, quoteCur, dateUTC);
            if (rate != null)
            {
                dicRate[cacheKey] = rate.Value;
                return rate;
            }
        }

        return null;
    }
}
public interface IExchangeRateTable
{
    decimal? GetRateFor(string baseCur, string quoteCur, DateTime date);
}
