using HR.BrightspaceConnector.Utilities;
using HR.Common.Utilities;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Infrastructure
{
    internal class PagedResultSetJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            var canBeConverted = typeToConvert.DerivesFromGenericParent(typeof(PagedResultSet<>));
            return canBeConverted;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions jsonOptions)
        {
            var converterType = typeof(PagedResultSetJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments().Single());
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }

        private class PagedResultSetJsonConverter<T> : JsonConverter<PagedResultSet<T>>
        {
            public override PagedResultSet<T>? Read(ref Utf8JsonReader jsonReader, Type typeToConvert, JsonSerializerOptions jsonOptions)
            {
                PagedResultSet<T> pagedResultSet = new();

                while (jsonReader.Read() && jsonReader.TokenType == JsonTokenType.PropertyName)
                {
                    var comparisonType = jsonOptions.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                    var propertyName = jsonReader.GetString();
                    
                    if (string.Equals(propertyName, typeToConvert.GetJsonPropertyName(nameof(PagedResultSet<T>.PagingInfo), jsonOptions.PropertyNamingPolicy), comparisonType))
                    {
                        pagedResultSet.PagingInfo = JsonSerializer.Deserialize<PagingInfo>(ref jsonReader, jsonOptions) ?? new PagingInfo();
                    }
                    else if (string.Equals(propertyName, typeToConvert.GetJsonPropertyName(nameof(PagedResultSet<T>.Items), jsonOptions.PropertyNamingPolicy), comparisonType))
                    {
                        pagedResultSet.Items = JsonSerializer.Deserialize<IEnumerable<T>>(ref jsonReader, jsonOptions) ?? Enumerable.Empty<T>();
                    }
                }

                if (jsonReader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException($"The JSON value could not be converted to {typeToConvert}.");
                }

                return pagedResultSet;
            }

            public override void Write(Utf8JsonWriter jsonWriter, PagedResultSet<T> pagedResultSet, JsonSerializerOptions jsonOptions)
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName(pagedResultSet.GetType().GetJsonPropertyName(nameof(PagedResultSet<T>.PagingInfo), jsonOptions.PropertyNamingPolicy));
                JsonSerializer.Serialize(jsonWriter, pagedResultSet.PagingInfo, jsonOptions);

                jsonWriter.WritePropertyName(pagedResultSet.GetType().GetJsonPropertyName(nameof(PagedResultSet<T>.Items), jsonOptions.PropertyNamingPolicy));
                JsonSerializer.Serialize(jsonWriter, pagedResultSet.Items, jsonOptions);

                jsonWriter.WriteEndObject();
            }
        }
    }
}
