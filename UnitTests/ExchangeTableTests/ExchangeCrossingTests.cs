namespace UnitTests.ExchangeTableTests;

using Simple.Finance.ExchangeRate;
using System;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// <see cref="ExchangeRateConverter.GetRateFor"/> walking a crossed route: rate accumulation,
/// per-hop table fallback, and memoisation. Uses <see cref="FakeRateTable"/> only, so the
/// numbers are exact and independent of the generated TemporalSeries data.
/// </summary>
public class ExchangeCrossingTests
{
    private static readonly DateTime date = new(2020, 12, 31);

    private static ExchangeRateConverter converterOf(params IExchangeRateTable[] tables)
    {
        var converter = new ExchangeRateConverter() { ExchangeRateTables = tables };
        converter.InitializeTables().Wait();
        return converter;
    }

    private static FakeRateTable brlTable() => new FakeRateTable("brl").Pair("BRL", "USD", 0.2m);
    private static FakeRateTable xauTable() => new FakeRateTable("xau").Pair("XAU", "USD", 2000m).Pair("XAU", "JPY", 200_000m);

    [Fact]
    public void GetRateFor_DirectHop_ReturnsTheTableRate()
    {
        var converter = converterOf(brlTable());

        Assert.Equal(0.2m, converter.GetRateFor("BRL", "USD", date));
    }

    [Fact]
    public void GetRateFor_InvertedHop_ReturnsTheReciprocal()
    {
        var converter = converterOf(brlTable());

        Assert.Equal(5m, converter.GetRateFor("USD", "BRL", date));
    }

    [Fact]
    public void GetRateFor_TwoHopCross_MultipliesBothHops()
    {
        var converter = converterOf(brlTable(), new FakeRateTable("eur").Pair("EUR", "USD", 1.25m));

        // BRL -> USD (0.2) -> EUR (1 / 1.25)
        Assert.Equal(0.16m, converter.GetRateFor("BRL", "EUR", date));
        Assert.Equal(6.25m, converter.GetRateFor("EUR", "BRL", date));
    }

    [Fact]
    public void GetRateFor_ThreeHopCross_MultipliesAndInvertsEachHop()
    {
        var converter = converterOf(brlTable(), xauTable());

        // BRL -> USD (0.2) -> XAU (1 / 2000) -> JPY (200 000)
        Assert.Equal(20m, converter.GetRateFor("BRL", "JPY", date));
        Assert.Equal(0.05m, converter.GetRateFor("JPY", "BRL", date));
    }

    [Fact]
    public void GetRateFor_CrossedRateIsTheInverseOfTheOppositeDirection()
    {
        var converter = converterOf(brlTable(), xauTable());

        var forward = converter.GetRateFor("JPY", "XAU", date);
        var backward = converter.GetRateFor("XAU", "JPY", date);

        Assert.Equal(1m, forward!.Value * backward!.Value);
    }

    [Theory]
    [InlineData("USD", "USD")]
    [InlineData("usd", "USD")]      // case-insensitive identity, resolved before any routing
    [InlineData("ZZZ", "ZZZ")]      // currency absent from every table
    public void GetRateFor_SameCurrency_ReturnsOneWithoutTouchingTheTables(string baseCur, string quoteCur)
    {
        var table = brlTable();
        var converter = converterOf(table);

        Assert.Equal(1m, converter.GetRateFor(baseCur, quoteCur, date));
        Assert.Empty(table.Queries);
    }

    [Theory]
    [InlineData("brl", "jpy")]
    [InlineData("BRL", "jpy")]
    [InlineData("bRl", "JpY")]
    public void GetRateFor_CrossesRegardlessOfTheCurrencyCasing(string baseCur, string quoteCur)
    {
        var converter = converterOf(brlTable(), xauTable());

        Assert.Equal(20m, converter.GetRateFor(baseCur, quoteCur, date));
    }

    [Fact]
    public void GetRateFor_UnreachableCurrency_ReturnsNull()
    {
        var converter = converterOf(brlTable(), new FakeRateTable("island").Pair("AAA", "BBB", 7m));

        Assert.Null(converter.GetRateFor("BRL", "AAA", date));
        Assert.Null(converter.GetRateFor("BRL", "ZZZ", date));
    }

    [Fact]
    public void GetRateFor_HopWithoutDataAnywhere_ReturnsNull()
    {
        // route exists (BRL -> USD -> XAU) but the second hop has no data on this date
        var converter = converterOf(brlTable(), new FakeRateTable("xau").Declare("XAU", "USD"));

        Assert.Null(converter.GetRateFor("BRL", "XAU", date));
        Assert.Equal(0.2m, converter.GetRateFor("BRL", "USD", date));  // the reachable part still works
    }

    [Fact]
    public void GetRateFor_HopTableWithoutDataForTheDate_FallsBackToAnotherTable()
    {
        var owner = new FakeRateTable("owner").Declare("BRL", "USD");
        var backup = new FakeRateTable("backup").Serve("BRL", "USD", 0.25m);

        var converter = converterOf(owner, backup);

        Assert.Equal(0.25m, converter.GetRateFor("BRL", "USD", date));
        Assert.Equal($"owner:BRL/USD@20201231", Assert.Single(owner.Queries));
        Assert.Equal($"backup:BRL/USD@20201231", Assert.Single(backup.Queries));
    }

    [Fact]
    public void GetRateFor_Fallback_TakesTheFirstTableInArrayOrder()
    {
        var owner = new FakeRateTable("owner").Declare("BRL", "USD");
        var cheap = new FakeRateTable("cheap").Serve("BRL", "USD", 0.25m);
        var pricey = new FakeRateTable("pricey").Serve("BRL", "USD", 0.30m);

        Assert.Equal(0.25m, converterOf(owner, cheap, pricey).GetRateFor("BRL", "USD", date));
        Assert.Equal(0.30m, converterOf(owner, pricey, cheap).GetRateFor("BRL", "USD", date));
    }

    [Fact]
    public void GetRateFor_Fallback_StopsAtTheFirstAnswerAndSkipsTheHopTable()
    {
        var owner = new FakeRateTable("owner").Declare("BRL", "USD");
        var backup = new FakeRateTable("backup").Serve("BRL", "USD", 0.25m);
        var unused = new FakeRateTable("unused").Serve("BRL", "USD", 0.30m);

        var converter = converterOf(owner, backup, unused);
        converter.GetRateFor("BRL", "USD", date);

        Assert.Single(owner.Queries);    // the graph table is never asked twice
        Assert.Single(backup.Queries);
        Assert.Empty(unused.Queries);    // search stops at the first table that answered
    }

    [Fact]
    public void GetRateFor_Fallback_AsksTablesInTheDeclaredOrientation()
    {
        var owner = new FakeRateTable("owner").Declare("BRL", "USD");
        var backup = new FakeRateTable("backup").Serve("BRL", "USD", 0.2m);

        var converter = converterOf(owner, backup);

        // crossing USD -> BRL still queries BRL/USD, the inversion happens after the lookup
        Assert.Equal(5m, converter.GetRateFor("USD", "BRL", date));
        Assert.Equal("owner:BRL/USD@20201231", Assert.Single(owner.Queries));
        Assert.Equal("backup:BRL/USD@20201231", Assert.Single(backup.Queries));
    }

    [Fact]
    public void GetRateFor_Fallback_AppliesPerHopNotPerRoute()
    {
        var owner = new FakeRateTable("owner").Declare("BRL", "USD").Pair("XAU", "USD", 2000m);
        var backup = new FakeRateTable("backup").Serve("BRL", "USD", 0.2m);

        var converter = converterOf(owner, backup);

        // hop 1 falls back to "backup", hop 2 is served by the graph table itself
        Assert.Equal(0.0001m, converter.GetRateFor("BRL", "XAU", date));
    }

    [Fact]
    public void GetRateFor_ZeroRateOnAnInvertedHop_ReturnsZeroInsteadOfDividingByZero()
    {
        var converter = converterOf(new FakeRateTable("btc").Pair("BTC", "USD", 0m));

        Assert.Equal(0m, converter.GetRateFor("USD", "BTC", date));
        Assert.Equal(0m, converter.GetRateFor("BTC", "USD", date));
    }

    [Fact]
    public void GetRateFor_MemoisesOnlyTheRequestedPair_NotTheIntermediateHops()
    {
        var brl = brlTable();
        var xau = xauTable();
        var cache = new SpyRateCache();
        var converter = converterOf(brl, xau);
        converter.CachingEngine = cache;

        Assert.Equal(20m, converter.GetRateFor("BRL", "JPY", date));
        var queriesAfterFirstCall = brl.Queries.Count + xau.Queries.Count;

        Assert.Equal(20m, converter.GetRateFor("BRL", "JPY", date));

        Assert.Equal(queriesAfterFirstCall, brl.Queries.Count + xau.Queries.Count);   // second call never hit a table

        var write = Assert.Single(cache.Writes);         // hops are not memoised, only the requested pair
        Assert.Equal("BRL/JPY#20201231", write.Key);
        Assert.Equal(20m, write.Value);
        Assert.Equal(2, cache.Reads.Count);
    }

    [Fact]
    public void GetRateFor_MemoisationIsPerDay_IgnoringTheTimeOfDay()
    {
        var table = new FakeRateTable("brl")
            .Declare("BRL", "USD")
            .ServeOn("BRL", "USD", new DateTime(2020, 12, 31), 0.2m)
            .ServeOn("BRL", "USD", new DateTime(2021, 1, 1), 0.3m);
        var converter = converterOf(table);

        Assert.Equal(0.2m, converter.GetRateFor("BRL", "USD", new DateTime(2020, 12, 31, 8, 0, 0)));
        Assert.Equal(0.2m, converter.GetRateFor("BRL", "USD", new DateTime(2020, 12, 31, 20, 30, 0)));
        Assert.Single(table.Queries);   // same day, served from cache

        Assert.Equal(0.3m, converter.GetRateFor("BRL", "USD", new DateTime(2021, 1, 1)));
        Assert.Equal(2, table.Queries.Count);   // another day, another lookup
    }

    [Fact]
    public void GetRateFor_CacheDisabled_RecrossesTheRouteEveryCall()
    {
        var brl = brlTable();
        var xau = xauTable();
        var cache = new SpyRateCache();
        var converter = converterOf(brl, xau);
        converter.CachingEngine = cache;
        converter.CacheEnabled = false;

        Assert.Equal(20m, converter.GetRateFor("BRL", "JPY", date));
        var queriesAfterFirstCall = brl.Queries.Count + xau.Queries.Count;
        Assert.Equal(20m, converter.GetRateFor("BRL", "JPY", date));

        Assert.Equal(queriesAfterFirstCall * 2, brl.Queries.Count + xau.Queries.Count);
        Assert.Empty(cache.Reads);
        Assert.Empty(cache.Writes);
    }

    [Fact]
    public void GetRateFor_FailedCrossing_IsNotMemoised()
    {
        var brl = brlTable();
        var xau = new FakeRateTable("xau").Declare("XAU", "USD");
        var cache = new SpyRateCache();
        var converter = converterOf(brl, xau);
        converter.CachingEngine = cache;

        Assert.Null(converter.GetRateFor("BRL", "XAU", date));
        Assert.Null(converter.GetRateFor("BRL", "XAU", date));

        Assert.Empty(cache.Writes);
        Assert.Equal(2, xau.Queries.Count);   // retried, so a table that gets data later is picked up
    }

    [Fact]
    public void GetRateFor_BeforeInitializeTables_ReturnsNull()
    {
        var converter = new ExchangeRateConverter() { ExchangeRateTables = [brlTable()] };

        Assert.Null(converter.GetRateFor("BRL", "USD", date));
    }

    [Fact]
    public void GetRateFor_WithoutTables_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new ExchangeRateConverter().GetRateFor("BRL", "USD", date));
    }

    [Fact]
    public async Task InitializeTables_WithoutTables_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(new ExchangeRateConverter().InitializeTables);
    }

    [Fact]
    public void InitializeTables_InitializesEveryTableOnce()
    {
        var brl = brlTable();
        var xau = xauTable();

        converterOf(brl, xau);

        Assert.Equal(1, brl.InitializeCalls);
        Assert.Equal(1, xau.InitializeCalls);
    }

    [Fact]
    public async Task InitializeTables_RebuildsTheGraph_ExposingTablesAddedLater()
    {
        var converter = converterOf(brlTable());
        Assert.Null(converter.GetRateFor("BRL", "JPY", date));

        converter.ExchangeRateTables = [.. converter.ExchangeRateTables, xauTable()];
        Assert.Null(converter.GetRateFor("BRL", "JPY", date));   // graph is still the one built before

        await converter.InitializeTables();
        Assert.Equal(20m, converter.GetRateFor("BRL", "JPY", date));
    }
}
