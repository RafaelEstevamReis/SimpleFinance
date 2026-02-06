namespace Simple.Finance.ExchangeRate;

using Simple.Finance.ExchangeRate.ExchangeTables;
using System;
using System.Collections.Generic;

public class ExchangeRateConverter
{
    public List<IExchangeRateTable> ExchangeRateTables { get; set; } = [];
    public IExchangeRateCaching CachingEngine { get; set; } = new ExchangeRateMemoryCache();

    /// <summary>
    /// Cache recent queries to avoid re-process same pairs on a date
    /// </summary>
    public bool CacheEnabled { get; set; } = true;

    public decimal? GetRateFor(string baseCur, string quoteCur, DateTime dateUTC)
    {
        if (ExchangeRateTables.Count == 0) throw new InvalidOperationException($"There are no {nameof(ExchangeRateTables)} to process");

        if (baseCur == quoteCur) return 1;

        decimal? rate;
        if (CacheEnabled)
        {
            rate = CachingEngine.GetCachedRateFor(baseCur, quoteCur, dateUTC);
            if (rate != null) return rate;
        }

        rate = getRateFor(baseCur, quoteCur, dateUTC);
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
            CachingEngine.SetCachedRateFor(baseCur, quoteCur, dateUTC, rate.Value);
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

    internal static decimal? getTableValue(decimal[][][] data, int firstYear, DateTime dt)
    {
        var ixYear = dt.Year - firstYear;
        if (ixYear >= data.GetLength(0)) return null;

        var ixMonh = dt.Month - 1;
        if (ixMonh >= data[ixYear].GetLength(0)) return null;

        var ixDay = dt.Day - 1;
        if (ixDay >= data[ixYear][ixMonh].Length) return null;

        return data[ixYear][ixMonh][ixDay];
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
