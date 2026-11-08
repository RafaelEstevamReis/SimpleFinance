namespace Simple.Finance.ExchangeRate;

using System;

public interface IExchangeRateCaching
{
    public decimal? GetCachedRateFor(string baseCur, string quoteCur, DateTime date);
    public void SetCachedRateFor(string baseCur, string quoteCur, DateTime date, decimal value);
}
