using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrediAvanzaAPI.Helpers
{
    public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private readonly string _serializationFormat;

        public DateOnlyJsonConverter(string? serializationFormat = null)
        {
            _serializationFormat = serializationFormat ?? "yyyy-MM-dd";
        }

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Unexpected token parsing DateOnly. Expected String, got {reader.TokenType}.");

            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
                throw new JsonException("DateOnly value was null or empty.");

            if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                return DateOnly.FromDateTime(dt);

            throw new JsonException($"Invalid DateOnly value: {value}");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_serializationFormat, CultureInfo.InvariantCulture));
        }
    }
}
