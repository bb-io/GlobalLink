using Newtonsoft.Json;

namespace Apps.GlobalLink.Utils;

public class UnixTimestampConverter : JsonConverter<DateTime>
{
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return DateTime.MinValue;

        if (reader.TokenType == JsonToken.Integer)
        {
            long timestamp = Convert.ToInt64(reader.Value);
            if (timestamp > 10000000000) 
            {
                return Epoch.AddMilliseconds(timestamp);
            }
            else
            {
                return Epoch.AddSeconds(timestamp);
            }
        }

        throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        if (value == DateTime.MinValue)
        {
            writer.WriteNull();
        }
        else
        {
            long milliseconds = (long)(value.ToUniversalTime() - Epoch).TotalMilliseconds;
            writer.WriteValue(milliseconds);
        }
    }

    public static ulong ToUnixTimestamp(DateTime dateTime)
    {
        return (ulong)(dateTime.ToUniversalTime() - Epoch).TotalMilliseconds;
    }
}
