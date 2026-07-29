namespace UnitTests.ExchangeTableTests;

using Simple.Finance.ExchangeRate;
using System;
using System.Linq;
using Xunit;

/// <summary>
/// BFS routing of <see cref="ExchangeGraph"/>: every declared pair becomes two edges
/// (forward + <c>Inverted</c>), and a route is the chain of hops crossing them.
/// </summary>
public class ExchangeGraphTests
{
    private static ExchangeGraph graphOf(params IExchangeRateTable[] tables) => new(tables);

    /// <summary>A route must be a contiguous chain: each hop starts where the previous one landed</summary>
    private static void assertChain(ExchangeGraph.Node[] route, string start, string target)
    {
        var current = start;
        foreach (var node in route)
        {
            if (node.Inverted)
            {
                Assert.Equal(node.QuoteCur, current, ignoreCase: true);
                current = node.BaseCur;
            }
            else
            {
                Assert.Equal(node.BaseCur, current, ignoreCase: true);
                current = node.QuoteCur;
            }
        }
        Assert.Equal(target, current, ignoreCase: true);
    }

    [Theory]
    [InlineData("USD", "USD")]
    [InlineData("usd", "USD")]
    [InlineData("ZZZ", "zzz")]  // unknown currency, still an identity
    public void GetRoute_SameCurrency_ReturnsEmptyRoute(string baseCur, string quoteCur)
    {
        var graph = graphOf(new FakeRateTable("t").Pair("BRL", "USD", 5m));

        Assert.Empty(graph.GetRoute(baseCur, quoteCur));
    }

    [Fact]
    public void GetRoute_DeclaredPair_ReturnsSingleForwardHop()
    {
        var table = new FakeRateTable("t").Pair("BRL", "USD", 0.2m);

        var route = graphOf(table).GetRoute("BRL", "USD");

        var hop = Assert.Single(route);
        Assert.Equal("BRL", hop.BaseCur);
        Assert.Equal("USD", hop.QuoteCur);
        Assert.False(hop.Inverted);
        Assert.Same(table, hop.Table);
    }

    [Fact]
    public void GetRoute_ReversedPair_ReturnsInvertedHopKeepingDeclaredOrientation()
    {
        var table = new FakeRateTable("t").Pair("BRL", "USD", 0.2m);

        var route = graphOf(table).GetRoute("USD", "BRL");

        var hop = Assert.Single(route);
        // orientation stays as declared by the table; only the flag marks the direction
        Assert.Equal("BRL", hop.BaseCur);
        Assert.Equal("USD", hop.QuoteCur);
        Assert.True(hop.Inverted);
        Assert.Same(table, hop.Table);
    }

    [Fact]
    public void GetRoute_TwoPairsSharingAHub_CrossesThroughTheHub()
    {
        var brl = new FakeRateTable("brl").Pair("BRL", "USD", 0.2m);
        var eur = new FakeRateTable("eur").Pair("EUR", "USD", 1.25m);

        var route = graphOf(brl, eur).GetRoute("BRL", "EUR");

        Assert.Equal(2, route.Length);
        assertChain(route, "BRL", "EUR");

        Assert.Same(brl, route[0].Table);
        Assert.False(route[0].Inverted);          // BRL -> USD

        Assert.Same(eur, route[1].Table);
        Assert.True(route[1].Inverted);           // USD -> EUR, crossing EUR/USD backwards
        Assert.Equal("EUR", route[1].BaseCur);
        Assert.Equal("USD", route[1].QuoteCur);
    }

    [Fact]
    public void GetRoute_ThreeHops_CrossesTwoHubs()
    {
        var brl = new FakeRateTable("brl").Pair("BRL", "USD", 0.2m);
        var xau = new FakeRateTable("xau").Pair("XAU", "USD", 2000m).Pair("XAU", "JPY", 200_000m);

        var route = graphOf(brl, xau).GetRoute("BRL", "JPY");

        Assert.Equal(3, route.Length);            // BRL -> USD -> XAU -> JPY
        assertChain(route, "BRL", "JPY");
        Assert.Equal(["BRL/USD", "XAU/USD", "XAU/JPY"], route.Select(o => $"{o.BaseCur}/{o.QuoteCur}"));
        Assert.Equal([false, true, false], route.Select(o => o.Inverted));
    }

    [Theory]
    [InlineData("BRL", "USD")]
    [InlineData("USD", "BRL")]
    [InlineData("BRL", "EUR")]
    [InlineData("EUR", "BRL")]
    [InlineData("BRL", "JPY")]
    [InlineData("JPY", "BRL")]
    [InlineData("JPY", "EUR")]
    [InlineData("CAD", "JPY")]
    [InlineData("JPY", "CAD")]
    [InlineData("SAT", "CAD")]
    [InlineData("CAD", "SAT")]
    [InlineData("brl", "jpy")]
    public void GetRoute_ChainIsContiguousAndReachesTarget(string baseCur, string quoteCur)
    {
        var route = fullGraph().GetRoute(baseCur, quoteCur);

        Assert.NotEmpty(route);
        assertChain(route, baseCur, quoteCur);
    }

    [Fact]
    public void GetRoute_IsCaseInsensitive()
    {
        var graph = graphOf(new FakeRateTable("t").Pair("BRL", "USD", 0.2m));

        Assert.Single(graph.GetRoute("brl", "usd"));
        Assert.Single(graph.GetRoute("Usd", "bRl"));
    }

    [Fact]
    public void GetRoute_PrefersFewestHops_EvenWhenTheLongPathIsDeclaredFirst()
    {
        var longWay = new FakeRateTable("long").Pair("AAA", "MID", 2m).Pair("MID", "BBB", 3m);
        var shortWay = new FakeRateTable("short").Pair("AAA", "BBB", 6m);

        var route = graphOf(longWay, shortWay).GetRoute("AAA", "BBB");

        var hop = Assert.Single(route);
        Assert.Same(shortWay, hop.Table);
    }

    [Fact]
    public void GetRoute_SamePairOnManyTables_UsesTheFirstDeclaringTable()
    {
        var first = new FakeRateTable("first").Pair("BRL", "USD", 0.2m);
        var second = new FakeRateTable("second").Pair("BRL", "USD", 0.3m);

        Assert.Same(first, Assert.Single(graphOf(first, second).GetRoute("BRL", "USD")).Table);
        Assert.Same(second, Assert.Single(graphOf(second, first).GetRoute("BRL", "USD")).Table);
    }

    [Fact]
    public void GetRoute_CyclicGraph_TerminatesOnTheShortestHop()
    {
        // A -> B -> C -> A closes a cycle: reaching C from A must not walk the long way nor loop
        var table = new FakeRateTable("t")
            .Pair("AAA", "BBB", 2m)
            .Pair("BBB", "CCC", 3m)
            .Pair("CCC", "AAA", 4m);

        var route = graphOf(table).GetRoute("AAA", "CCC");

        var hop = Assert.Single(route);
        Assert.True(hop.Inverted);
        Assert.Equal("CCC", hop.BaseCur);
        Assert.Equal("AAA", hop.QuoteCur);
    }

    [Fact]
    public void GetRoute_DisconnectedIsland_ReturnsEmptyRoute()
    {
        var fiat = new FakeRateTable("fiat").Pair("BRL", "USD", 0.2m).Pair("EUR", "USD", 1.25m);
        var island = new FakeRateTable("island").Pair("AAA", "BBB", 7m);

        var graph = graphOf(fiat, island);

        Assert.Empty(graph.GetRoute("BRL", "AAA"));
        Assert.Empty(graph.GetRoute("AAA", "EUR"));
        Assert.Single(graph.GetRoute("AAA", "BBB"));  // still routable inside its own island
    }

    [Theory]
    [InlineData("ZZZ", "USD")]  // unknown start
    [InlineData("USD", "ZZZ")]  // unknown target
    public void GetRoute_UnknownCurrency_ReturnsEmptyRoute(string baseCur, string quoteCur)
    {
        Assert.Empty(fullGraph().GetRoute(baseCur, quoteCur));
    }

    [Fact]
    public void GetRoute_WithoutTables_ReturnsEmptyRoute()
    {
        Assert.Empty(graphOf().GetRoute("BRL", "USD"));
    }

    [Fact]
    public void GetRoute_HopCarriesTheTableThatDeclaredIt()
    {
        var brl = new FakeRateTable("brl").Pair("BRL", "USD", 0.2m);
        var xau = new FakeRateTable("xau").Pair("XAU", "USD", 2000m).Pair("XAU", "JPY", 200_000m);

        var route = graphOf(brl, xau).GetRoute("JPY", "BRL");

        Assert.Equal([xau, xau, brl], route.Select(o => o.Table));
    }

    /// <summary>Mirrors the shape of the real tables: USD and XAU are the hubs</summary>
    private static ExchangeGraph fullGraph() => graphOf(
        new FakeRateTable("brl").Pair("BRL", "USD", 0.2m),
        new FakeRateTable("eur").Pair("EUR", "USD", 1.25m),
        new FakeRateTable("btc").Pair("BTC", "USD", 20_000m).Pair("SAT", "USD", 0.0002m),
        new FakeRateTable("xau").Pair("XAU", "USD", 2000m)
                                .Pair("XAU", "JPY", 200_000m)
                                .Pair("XAU", "CAD", 2500m));
}
