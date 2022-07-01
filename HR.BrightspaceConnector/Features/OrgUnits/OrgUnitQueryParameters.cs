using HR.BrightspaceConnector.Utilities;

using System.Text;
using System.Text.Json;

namespace HR.BrightspaceConnector.Features.OrgUnits
{
    public class OrgUnitQueryParameters
    {
        /// <summary>
        /// Optional. Filter to org units with type matching this org unit type ID.
        /// </summary>
        public int? OrgUnitType { get; set; }

        /// <summary>
        /// Optional. Filter to org units with codes containing this substring.
        /// </summary>
        public string? OrgUnitCode { get; set; }

        /// <summary>
        /// Optional. Filter to org units with names containing this substring.
        /// </summary>
        public string? OrgUnitName { get; set; }

        /// <summary>
        /// Optional. Bookmark to use for fetching next data set segment.
        /// </summary>
        public string? Bookmark { get; set; }

        /// <summary>
        /// Optional. Filter to org units with codes precisely matching this string. Overrides orgUnitCode.
        /// </summary>
        public string? ExactOrgUnitCode { get; set; }

        /// <summary>
        /// Optional. Filter to org units with names precisely matching this string. Overrides orgUnitName.
        /// </summary>
        public string? ExactOrgUnitName { get; set; }

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
