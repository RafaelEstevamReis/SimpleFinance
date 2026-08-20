namespace Simple.Finance.WebApi.DataConverters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Nullable counterpart of <see cref="UtcDateTimeConverter"/>
/// </summary>
internal class NullableUtcDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Null ? null : UtcDateTimeConverter.toUtc(reader.GetDateTime());

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(UtcDateTimeConverter.toUtc(value.Value));
    }
}
