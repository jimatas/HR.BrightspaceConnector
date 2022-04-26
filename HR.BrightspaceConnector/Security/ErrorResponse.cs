using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Security
{
    internal class ErrorResponse
    {
        [JsonPropertyName("error")]
        public string? ErrorCode { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }

        [JsonPropertyName("rfc6749_reference")]
        public string? Rfc6749Reference { get; set; }
    }
}
