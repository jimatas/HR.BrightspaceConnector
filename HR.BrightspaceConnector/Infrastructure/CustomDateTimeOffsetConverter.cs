using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Infrastructure
{
    internal class CustomDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
    {
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff'Z'";

        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString() is string value
                && DateTimeOffset.TryParseExact(
                    value,
                    DateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTimeOffset result)
                ? result
                : null;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(((DateTimeOffset)value).ToString(DateTimeFormat, CultureInfo.InvariantCulture));
            }
        }
    }
}
