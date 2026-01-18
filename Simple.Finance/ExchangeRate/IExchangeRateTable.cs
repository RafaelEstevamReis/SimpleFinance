namespace Simple.Finance.ExchangeRate;

using System;

public interface IExchangeRateTable
{
    decimal? GetRateFor(string baseCur, string quoteCur, DateTime date);
}
