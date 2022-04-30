using HR.BrightspaceConnector.Utilities;

using System.Text;
using System.Text.Json;

namespace HR.BrightspaceConnector.Features.Users
{
    public class UserQueryParameters
    {
        /// <summary>
        /// Optional. Org-defined identifier to look for.
        /// </summary>
        public string? OrgDefinedId { get; set; }

        /// <summary>
        /// Optional. User name to look for.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Optional. External email address to look for.
        /// </summary>
        public string? ExternalEmail { get; set; }

        /// <summary>
        /// Optional. Bookmark to use for fetching next data set segment.
        /// </summary>
        public string? Bookmark { get; set; }

        public string ToQueryString(JsonNamingPolicy? propertyNamingPolicy = null)
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
