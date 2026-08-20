namespace Simple.Finance.WebApi.DataConverters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Every date in this service is UTC, but SQLite gives them back as
/// <see cref="DateTimeKind.Unspecified"/>. This forces the wire format to be
/// explicit UTC ('Z'), so clients never read a UTC value as local time
/// </summary>
internal class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => toUtc(reader.GetDateTime());

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(toUtc(value));

    internal static DateTime toUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    };
}
