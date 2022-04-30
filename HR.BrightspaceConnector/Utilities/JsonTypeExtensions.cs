using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Utilities
{
    internal static class JsonTypeExtensions
    {
        public static string GetJsonPropertyName(this Type target, string propertyName, JsonNamingPolicy? propertyNamingPolicy = null)
        {
            var property = target.GetProperty(propertyName);
            return property?.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                ?? propertyNamingPolicy?.ConvertName(propertyName)
                ?? propertyName;
        }
    }
}
