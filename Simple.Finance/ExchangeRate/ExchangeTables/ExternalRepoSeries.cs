namespace Simple.Finance.ExchangeRate.ExchangeTables;

using System;
using System.Collections.Generic;
using System.Text;

public class ExternalRepoSeries
{
}
public class ExternalDataModel
{
    public string QuoteCurrency { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = string.Empty;
    public DateTime FileGenerationDate { get; set; }

    public Dictionary<int, Dictionary<string, decimal>> Values { get; set; } = [];
}
