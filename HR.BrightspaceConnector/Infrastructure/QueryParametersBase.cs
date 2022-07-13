using HR.BrightspaceConnector.Utilities;

using System.Text;
using System.Text.Json;

namespace HR.BrightspaceConnector.Infrastructure
{
    public abstract class QueryParametersBase
    {
        /// <summary>
        /// Returns a string that can be used as the query part of a URL and consists of as many key/value pairs as there are public properties defined on the object.
        /// </summary>
        /// <remarks>
        /// Note that the default implementation simply uses the result obtained by calling ToString() on the property value to construct each key/value pair, as long as that value is not null or an empty string.
        /// </remarks>
        /// <param name="propertyNamingPolicy"></param>
        /// <returns></returns>
        public virtual string ToQueryString(JsonNamingPolicy? propertyNamingPolicy = null)
        {
            StringBuilder queryStringBuilder = new();
            var separator = '?';

            foreach (var property in GetType().GetProperties())
            {
                var propertyValue = property.GetValue(this)?.ToString();
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    var propertyName = GetType().GetJsonPropertyName(property.Name, propertyNamingPolicy);
                    queryStringBuilder.Append(separator).Append(propertyName).Append('=').Append(Uri.EscapeDataString(propertyValue.ToString()));
                    separator = '&';
                }
            }

            return queryStringBuilder.ToString();
        }
    }
}
