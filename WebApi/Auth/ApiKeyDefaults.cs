namespace Simple.Finance.WebApi.Auth;

/// <summary>
/// Names of the Key authentication, the single place where they are defined
/// </summary>
public static class ApiKeyDefaults
{
    /// <summary>
    /// Authentication scheme name, also the id of the OpenAPI security definition
    /// </summary>
    public const string Scheme = "ApiKey";
    /// <summary>
    /// Request header carrying the account Key
    /// </summary>
    public const string HeaderName = "X-Api-Key";
    /// <summary>
    /// Claim holding the account Key of the authenticated request
    /// </summary>
    public const string AccountKeyClaim = "account_key";
}
