namespace UnitTests.ExchangeTableTests;

using Simple.Finance.ExchangeRate;
using System;
using Xunit;

public class CurrencyPairsTest
{
    private readonly ExchangeRateConverter exchange;

    public CurrencyPairsTest()
    {
        exchange = ExchangeRateConverter.CreateWithTemporalSeries();
        var task = exchange.InitializeTables();
        task.Wait();
    }

    [Fact]
    public void SimplePairTest_BTC_USD_20201231()
    {
        var expected = 29027.0M;
        var actual = exchange.GetRateFor("BTC", "USD", new DateTime(2020, 12, 31));

        Assert.Equal(expected, actual);
    }
    [Fact]
    public void SimplePairTest_SATs_USD_20201231()
    {
        var expected = 0.00029027M;
        var actual = exchange.GetRateFor("SAT", "USD", new DateTime(2020, 12, 31));

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("BRL", "USD", 0.1924520313)]
    [InlineData("USD", "BRL", 5.1961000000)]
    [InlineData("EUR", "USD", 1.2289062500)]
    [InlineData("USD", "EUR", 0.8137317228)]
    [InlineData("BTC", "USD", 29027.000000)]
    [InlineData("USD", "BTC", 0.0000344507)]
    [InlineData("XAU", "USD", 1887.6000000)]
    [InlineData("USD", "XAU", 0.0005297733)]
    [InlineData("XAU", "JPY", 194895.00000)]
    [InlineData("JPY", "XAU", 0.0000051310)]
    [InlineData("XAU", "EUR", 1536.0000000)]
    [InlineData("EUR", "XAU", 0.0006510417)]
    public void DirectPairTestAt20201231(string baseCur, string quoteCur, decimal expected)
    {
        var actual = exchange.GetRateFor(baseCur, quoteCur, new DateTime(2020, 12, 31));
        ArgumentNullException.ThrowIfNull(actual);

        Assert.Equal(expected, (decimal)actual, precision: expected > 100_000 ? 6 : 10);
    }

    [Theory]
    [InlineData("BRL", "EUR", 0.1566043230)]     // BRL → USD → EUR
    [InlineData("EUR", "BRL", 6.3855197656)]
    [InlineData("BRL", "BTC", 0.0000066301)]     // BRL → USD → BTC
    [InlineData("BTC", "BRL", 150827.19470)]
    [InlineData("JPY", "BTC", 0.0000003337)]     // JPY → XAU → USD → BTC (ou JPY→XAU→USD)
    [InlineData("BTC", "JPY", 2997042.363318)]
    [InlineData("EUR", "BTC", 0.0000423367)]     // EUR → USD → BTC
    [InlineData("BRL", "XAU", 0.0001019559)]     // BRL → USD → XAU
    [InlineData("XAU", "BRL", 9808.1583600)]
    [InlineData("JPY", "EUR", 0.0078811668)]     // JPY → XAU → EUR
    [InlineData("GBP", "BTC", 0.0000468509)]     // GBP → XAU → USD → BTC
    [InlineData("CAD", "XAU", 0.0004149550)]     // CAD → XAU
    [InlineData("XAU", "CAD", 2409.9000000)]
    public void IndirectPairTestAt20201231(string baseCur, string quoteCur, decimal expected)
    {
        var actual = exchange.GetRateFor(baseCur, quoteCur, new DateTime(2020, 12, 31));
        ArgumentNullException.ThrowIfNull(actual);

        Assert.Equal(expected, (decimal)actual, precision: expected > 100_000 ? 6 : 10);
    }

}
