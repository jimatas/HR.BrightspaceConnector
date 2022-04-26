using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Security
{
    public class CacheableTokenResponse : TokenResponse
    {
        public CacheableTokenResponse() : this(SystemClock.Instance) { }
        public CacheableTokenResponse(IClock clock) => CreatedAt = clock.Now;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// The timestamp at which the token expires. Calculated from the creation timestamp and TTL.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset ExpiresAt => CreatedAt + (ExpiresIn ?? TimeSpan.Zero);
    }
}
