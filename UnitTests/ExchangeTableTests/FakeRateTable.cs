namespace UnitTests.ExchangeTableTests;

using Simple.Finance.ExchangeRate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Deterministic in-memory <see cref="IExchangeRateTable"/>.
/// Keeps "which pairs the graph sees" (<see cref="Declare"/>) apart from
/// "which pairs actually answer on a date" (<see cref="Serve"/>), so the per-hop
/// table fallback of the converter can be exercised without any data file.
/// </summary>
internal sealed class FakeRateTable(string name) : IExchangeRateTable
{
    private readonly List<(string, string)> pairs = [];
    private readonly Dictionary<string, Func<DateTime, decimal?>> rates = new(StringComparer.OrdinalIgnoreCase);

    public string Name { get; } = name;
    public int InitializeCalls { get; private set; }

    /// <summary>Every GetRateFor received, as "Name:BASE/QUOTE@yyyyMMdd"</summary>
    public List<string> Queries { get; } = [];

    /// <summary>Publishes the pair to the graph without ever returning data (a series that does not cover the date)</summary>
    public FakeRateTable Declare(string baseCur, string quoteCur)
    {
        pairs.Add((baseCur, quoteCur));
        return this;
    }
    /// <summary>Answers the pair on any date, without publishing it to the graph (fallback-only source)</summary>
    public FakeRateTable Serve(string baseCur, string quoteCur, decimal rate)
    {
        rates[key(baseCur, quoteCur)] = _ => rate;
        return this;
    }
    /// <summary>Answers the pair only on <paramref name="date"/> (day granularity), without publishing it</summary>
    public FakeRateTable ServeOn(string baseCur, string quoteCur, DateTime date, decimal rate)
    {
        if (!rates.TryGetValue(key(baseCur, quoteCur), out var previous)) previous = _ => null;
        rates[key(baseCur, quoteCur)] = dt => dt.Date == date.Date ? rate : previous(dt);
        return this;
    }
    /// <summary><see cref="Declare"/> + <see cref="Serve"/>: a table that owns and answers a pair</summary>
    public FakeRateTable Pair(string baseCur, string quoteCur, decimal rate)
        => Declare(baseCur, quoteCur).Serve(baseCur, quoteCur, rate);

    public decimal? GetRateFor(string baseCur, string quoteCur, DateTime date)
    {
        Queries.Add($"{Name}:{baseCur}/{quoteCur}@{date:yyyyMMdd}");

        if (!rates.TryGetValue(key(baseCur, quoteCur), out var resolver)) return null;
        return resolver(date);
    }
    public IEnumerable<(string, string)> AvailableCurrencyPairs() => pairs;

    public Task Initialize()
    {
        InitializeCalls++;
        return Task.CompletedTask;
    }

    private static string key(string baseCur, string quoteCur) => $"{baseCur}/{quoteCur}";
}

/// <summary>
/// Wraps the production <see cref="ExchangeRateMemoryCache"/> recording every hit,
/// to assert *which* pairs get memoised (endpoints only, never the intermediate hops).
/// </summary>
internal sealed class SpyRateCache : IExchangeRateCaching
{
    private readonly ExchangeRateMemoryCache inner = new();

    public List<string> Reads { get; } = [];
    public List<(string Key, decimal Value)> Writes { get; } = [];

    public decimal? GetCachedRateFor(string baseCur, string quoteCur, DateTime date)
    {
        Reads.Add(key(baseCur, quoteCur, date));
        return inner.GetCachedRateFor(baseCur, quoteCur, date);
    }
    public void SetCachedRateFor(string baseCur, string quoteCur, DateTime date, decimal value)
    {
        Writes.Add((key(baseCur, quoteCur, date), value));
        inner.SetCachedRateFor(baseCur, quoteCur, date, value);
    }

    private static string key(string baseCur, string quoteCur, DateTime date) => $"{baseCur}/{quoteCur}#{date:yyyyMMdd}";
}
