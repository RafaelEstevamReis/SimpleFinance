namespace Simple.Finance.WebApi;

using System.Reflection;

/// <summary>
/// Identity of this service, reported by the OpenAPI document and by /api/hello
/// </summary>
public static class ApiInfo
{
    public const string Title = "Simple.Finance API";

    /// <summary>
    /// Assembly version of this service
    /// </summary>
    public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.0.0";
}
