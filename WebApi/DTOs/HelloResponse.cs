namespace Simple.Finance.WebApi.DTOs;

using System;

/// <summary>
/// Answer of the public liveness endpoint
/// </summary>
public record HelloResponse
{
    /// <summary>
    /// Fixed greeting
    /// </summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>
    /// Service name
    /// </summary>
    public string Service { get; set; } = string.Empty;
    /// <summary>
    /// Assembly version of the running service
    /// </summary>
    public string Version { get; set; } = string.Empty;
    /// <summary>
    /// Server time, always UTC
    /// </summary>
    public DateTime UtcNow { get; set; }
}
