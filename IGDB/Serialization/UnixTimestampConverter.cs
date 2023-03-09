using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IGDB
{
    public class UnixTimestampConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out var parsedUnixTimestamp))
                {
                    try
                    {
                        return DateTimeOffset.FromUnixTimeSeconds(parsedUnixTimestamp);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // it's invalid
                    }
                }
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ToUnixTimeSeconds());
        }
    }
}