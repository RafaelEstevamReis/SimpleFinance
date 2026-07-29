namespace Simple.Finance.ExchangeRate;

using Simple.Finance.ExchangeRate.ExchangeTables;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ExchangeRateConverter
{
    public IExchangeRateTable[] ExchangeRateTables { get; set; } = [];
    public IExchangeRateCaching CachingEngine { get; set; } = new ExchangeRateMemoryCache();

    /// <summary>
    /// Cache recent queries to avoid re-process same pairs on a date
    /// </summary>
    public bool CacheEnabled { get; set; } = true;

    private ExchangeGraph exchangeGraph = new([]);

    public async Task InitializeTables()
    {
        if (ExchangeRateTables.Length == 0) throw new InvalidOperationException($"There are no {nameof(ExchangeRateTables)} to process");

        var allTaks = ExchangeRateTables
            .Select(o => o.Initialize())
            .ToArray();
        await Task.WhenAll(allTaks);

        exchangeGraph = new ExchangeGraph(ExchangeRateTables);
    }

    public decimal? GetRateFor(string baseCur, string quoteCur, DateTime dateUTC)
    {
        if (ExchangeRateTables.Length == 0) throw new InvalidOperationException($"There are no {nameof(ExchangeRateTables)} to process");

        if (baseCur == quoteCur) return 1;

        decimal? rate;
        if (CacheEnabled)
        {
            rate = CachingEngine.GetCachedRateFor(baseCur, quoteCur, dateUTC);
            if (rate != null) return rate;
        }

        rate = getRateFor(exchangeGraph, ExchangeRateTables, baseCur, quoteCur, dateUTC);

        if (CacheEnabled && rate != null)
        {
            CachingEngine.SetCachedRateFor(baseCur, quoteCur, dateUTC, rate.Value);
        }

        return rate;
    }
    protected static decimal? getRateFor(ExchangeGraph exchangeGraph, IExchangeRateTable[] exchangeRateTables, string baseCur, string quoteCur, DateTime dateUTC)
    {
        if (string.Equals(baseCur, quoteCur, StringComparison.InvariantCultureIgnoreCase)) return 1.0m;

        // 1. Obtém o caminho de conversão
        var path = exchangeGraph.GetRoute(baseCur, quoteCur);
        if (path.Length == 0) return null;

        decimal? currentRate = 1.0m;

        foreach (var node in path)
        {
            string callBase = node.BaseCur;
            string callQuote = node.QuoteCur;

            decimal? rate = null;

            // 2. Estratégia de busca por tabela válida para a data
            // Tenta primeiro a tabela que veio do grafo (mais provável)
            if (node.Table != null)
            {
                rate = node.Table.GetRateFor(callBase, callQuote, dateUTC);
            }

            // Se falhar (null), tenta as demais tabelas na ordem do array
            if (rate is null)
            {
                foreach (var table in exchangeRateTables)
                {
                    // Evita tentar a mesma tabela novamente
                    if (table == node.Table) continue;

                    rate = table.GetRateFor(callBase, callQuote, dateUTC);
                    if (rate.HasValue) break; // Pegamos a primeira que retornou valor
                }
            }

            if (!rate.HasValue) return null; // Não foi possível converter esta etapa

            // 3. Acumula a taxa
            currentRate = currentRate * (node.Inverted && rate.Value != 0 ? (1 / rate.Value) : rate.Value);
        }

        return currentRate;
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
