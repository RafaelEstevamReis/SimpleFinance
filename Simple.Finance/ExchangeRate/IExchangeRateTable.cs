namespace Simple.Finance.ExchangeRate;

using System;
using System.Threading.Tasks;

public interface IExchangeRateTable
{
    Task Initialize();
    decimal? GetRateFor(string baseCur, string quoteCur, DateTime date);
}
