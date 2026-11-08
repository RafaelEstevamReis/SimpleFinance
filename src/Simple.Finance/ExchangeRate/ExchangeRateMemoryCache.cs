namespace Simple.Finance.ExchangeRate;

using System;
using System.Collections.Generic;

public class ExchangeRateMemoryCache : IExchangeRateCaching
{
    Dictionary<string, decimal> dicRate = [];

    public decimal? GetCachedRateFor(string baseCur, string quoteCur, DateTime date)
    {
        var cacheKey = getCacheKey(baseCur, quoteCur, date);
        if (dicRate.ContainsKey(cacheKey)) return dicRate[cacheKey];
        return null;
    }
    public void SetCachedRateFor(string baseCur, string quoteCur, DateTime date, decimal value)
    {
        var cacheKey = getCacheKey(baseCur, quoteCur, date);
        dicRate[cacheKey] = value;
    }
    private static string getCacheKey(string baseCur, string quoteCur, DateTime dateUTC) => $"{baseCur}/{quoteCur}#{dateUTC:yyyyMMdd}";
}