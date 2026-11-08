namespace Simple.Finance.ExchangeRate.ExchangeTables;

using Simple.API;
using Simple.API.ClientBuilderAttributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class ExternalRepoSeries : IExchangeRateTable
{
    private readonly IExternalSeries client;
    private readonly List<ExternalDataModel> models = [];

    public ExternalRepoSeries()
    {
        client = ClientBuilder.Create<IExternalSeries>("https://raw.githubusercontent.com/RafaelEstevamReis/SimpleFinance/refs/heads/main/Assets/");
    }

    public async Task Initialize()
    {
        Task<ExternalDataModel>[] tasks = [
            client.GetData_XAUUSD(),
            client.GetData_XAUGBP(),
            client.GetData_XAUEUR(),

            client.GetData_BTCUSD(),
            client.GetData_SATUSD(),

            client.GetData_USDBRL()
        ];

        var results = await Task.WhenAll(tasks);

        models.AddRange(results);
    }

    public IEnumerable<(string, string)> AvailableCurrencyPairs()
    {
        return models.Select(o => (o.BaseCurrency, o.QuoteCurrency));
    }

    public decimal? GetRateFor(string baseCur, string quoteCur, DateTime date)
    {
        var model = models.FirstOrDefault(o => o.BaseCurrency == baseCur && o.QuoteCurrency == quoteCur);
        if (model == null) return null;

        if (!model.Values.TryGetValue(date.Year, out Dictionary<string, decimal> year)) return null;

        var key = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        if (year.TryGetValue(key, out decimal value)) return value;
        return null;
    }

    public interface IExternalSeries
    {
        [Get("TemporalSeries_XAUUSD.json")]
        Task<ExternalDataModel> GetData_XAUUSD();
        [Get("TemporalSeries_XAUGBP.json")]
        Task<ExternalDataModel> GetData_XAUGBP();
        [Get("TemporalSeries_XAUEUR.json")]
        Task<ExternalDataModel> GetData_XAUEUR();

        [Get("TemporalSeries_BTCUSD.json")]
        Task<ExternalDataModel> GetData_BTCUSD();
        [Get("TemporalSeries_SATUSD.json")]
        Task<ExternalDataModel> GetData_SATUSD();

        [Get("TemporalSeries_USDBRL.json")]
        Task<ExternalDataModel> GetData_USDBRL();
    }
}
public class ExternalDataModel
{
    public string QuoteCurrency { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = string.Empty;
    public DateTime FileGenerationDate { get; set; }

    public Dictionary<int, Dictionary<string, decimal>> Values { get; set; } = [];
}
