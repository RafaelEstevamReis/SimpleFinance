namespace Simple.Finance.WebApi.DTOs;

/// <summary>
/// New value of a single preference
/// </summary>
public record PreferenceRequest
{
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// A preference of the account, stored on the management database
/// </summary>
public record PreferenceResponse
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public static PreferenceResponse From(AccountManagement.AccountPreference preference) => new()
    {
        Name = preference.Name,
        Value = preference.Value,
    };
}
