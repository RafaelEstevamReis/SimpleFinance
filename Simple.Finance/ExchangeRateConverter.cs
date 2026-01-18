namespace Simple.Finance;

using Simple.Finance.ExchangeTables;
using System;
using System.Collections.Generic;

public class ExchangeRateConverter
{
    protected Dictionary<string, decimal> dicRate = [];
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

        var rate = getRateFor(baseCur, quoteCur, dateUTC);

        if (rate == null)
        {
            // Try to get inverted
            var invertedRate = getRateFor(quoteCur, baseCur, dateUTC);
            if (invertedRate != null && invertedRate != 0)
            {
                rate = 1 / invertedRate;
            }
        }

        if (CacheEnabled && rate != null)
        {
            dicRate[cacheKey] = rate.Value;
        }

        return rate;
    }
    protected decimal? getRateFor(string baseCur, string quoteCur, DateTime dateUTC)
    {
        foreach (var converter in ExchangeRateTables)
        {
            var rate = converter.GetRateFor(baseCur, quoteCur, dateUTC);
            if (rate != null)
            {
                return rate;
            }
        }
        return null;
    }

    public static ExchangeRateConverter CreateWithTemporalSeries()
    {
        var exRate = new ExchangeRateConverter()
        {
            ExchangeRateTables = [
                new TemporalSeries_BRLUSD(),
                new TemporalSeries_EURUSD(),
                new TemporalSeries_BTCUSD(),

                new TemporalSeries_XAUCAD(),
                new TemporalSeries_XAUCHF(),
                new TemporalSeries_XAUCNY(),
                new TemporalSeries_XAUEUR(),
                new TemporalSeries_XAUGBP(),
                new TemporalSeries_XAUJPY(),
                new TemporalSeries_XAUUSD(),
            ],
        };
        return exRate;
    }
}
public interface IExchangeRateTable
{
    decimal? GetRateFor(string baseCur, string quoteCur, DateTime date);
}
