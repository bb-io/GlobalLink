using Newtonsoft.Json;

namespace Apps.GlobalLink.Utils;

public class UnixTimestampConverter : JsonConverter
{
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            if (objectType == typeof(DateTime?))
            {
                return null!;
            }

            return DateTime.MinValue;
        }

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

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null || (value is DateTime dateTime && dateTime == DateTime.MinValue))
        {
            writer.WriteNull();
        }
        else
        {
            DateTime currentDateTime = (DateTime)value;
            long milliseconds = (long)(currentDateTime.ToUniversalTime() - Epoch).TotalMilliseconds;
            writer.WriteValue(milliseconds);
        }
    }

    public static ulong ToUnixTimestamp(DateTime dateTime)
    {
        return (ulong)(dateTime.ToUniversalTime() - Epoch).TotalMilliseconds;
    }
}
