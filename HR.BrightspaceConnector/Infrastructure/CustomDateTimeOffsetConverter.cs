using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Infrastructure
{
    internal class CustomDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public override DateTimeOffset Read(ref Utf8JsonReader jsonReader, Type typeToConvert, JsonSerializerOptions jsonOptions)
        {
            return DateTimeOffset.TryParseExact(
                jsonReader.GetString(),
                DateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTimeOffset value) ? value : throw new JsonException($"The JSON value could not be converted to {typeof(DateTimeOffset)}.");
        }

        public override void Write(Utf8JsonWriter jsonWriter, DateTimeOffset value, JsonSerializerOptions jsonOptions)
        {
            jsonWriter.WriteStringValue(value.ToUniversalTime().ToString(DateTimeFormat, CultureInfo.InvariantCulture));
        }
    }
}
