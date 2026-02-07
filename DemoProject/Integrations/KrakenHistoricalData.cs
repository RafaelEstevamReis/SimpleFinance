namespace DemoProject.Integrations;

using Simple.API;
using Simple.API.ClientBuilderAttributes;
using Simple.Finance.ExchangeRate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class KrakenHistoricalData : IExchangeRateTable
{
    private IKrakenProvider krakenClient;
    private DateTime minDate;
    private Dictionary<string, decimal> valueCache;

    public KrakenHistoricalData()
    {
        krakenClient = ClientBuilder.Create<IKrakenProvider>("https://api.kraken.com/");
        minDate = DateTime.UtcNow.AddMonths(-18); // Max OHLC data
        valueCache = [];
    }

    public async Task Initialize()
    {
        var result = await krakenClient.GetOHLC("XBTUSD", 1440);

        foreach (var candle in result.result.XXBTZUSD)
        {
            var timestamp = DateTime.UnixEpoch.AddSeconds((int)candle[0]);
            var closePrice = candle[4];

            string key = timestamp.ToString("yyyy-MM-dd");
            valueCache[key] = closePrice;
        }
    }

    public decimal? GetRateFor(string baseCur, string quoteCur, DateTime date)
    {
        if (date >= DateTime.UtcNow.Date) return null; // Future Value
        if (date < minDate) return null;

        if (quoteCur != "USD") return null;

        if (baseCur == "BTC") return getValueFor(date);
        if (baseCur == "SAT")
        {
            return getValueFor(date) / 100_000_000M;
        }

        return null;
    }

    private decimal? getValueFor(DateTime date)
    {
        string key = date.ToString("yyyy-MM-dd");
        if (valueCache.TryGetValue(key, out var value)) return value;
        return null;
    }

    internal interface IKrakenProvider
    {
        [Get("/0/public/OHLC?pair={pair}&interval={interval}&since=")]
        Task<OHLCResult> GetOHLC([InRoute] string pair, [InRoute] int interval);
    }

    public class OHLCResult
    {
        public string[] error { get; set; } = [];

        public HistoricalData result { get; set; } = null!;
    }
    public class HistoricalData
    {
        public decimal[][] XXBTZUSD { get; set; }
        public long Last { get; set; }
    }
}

