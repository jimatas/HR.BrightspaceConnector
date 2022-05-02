using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Utilities
{
    internal static class JsonTypeExtensions
    {
        /// <summary>
        /// Returns the JSON name of a property, which is either the name that is configured through a possible <see cref="JsonPropertyNameAttribute"/>, or 
        /// if not present, the name returned by the optionally specified <see cref="JsonNamingPolicy"/>, or failing that, the supplied property name as is.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyNamingPolicy"></param>
        /// <returns></returns>
        public static string GetJsonPropertyName(this Type target, string propertyName, JsonNamingPolicy? propertyNamingPolicy = null)
        {
            var property = target.GetProperty(propertyName);
            return property?.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                ?? propertyNamingPolicy?.ConvertName(propertyName)
                ?? propertyName;
        }
    }
}
